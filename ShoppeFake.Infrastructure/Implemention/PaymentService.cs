using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayOS;
using PayOS.Exceptions;

using PayOS.Models.Webhooks;
using ShoppeFake.Application.DTOs.PaymentDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Infrastructure.Implemention
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            IConfiguration configuration,
            ILogger<PaymentService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Result<PaymentLinkResponse>> CreatePaymentLinkFromCartAsync(CheckoutPaymentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ReceiverName)
                || string.IsNullOrWhiteSpace(request.ReceiverPhone)
                || string.IsNullOrWhiteSpace(request.ShippingAddress))
            {
                return Result<PaymentLinkResponse>.Fail("INVALID_CHECKOUT", "Receiver information is required.");
            }

            var userResult = await _userService.GetUserLoginsAsync();
            if (userResult.Value == null)
            {
                return Result<PaymentLinkResponse>.Fail(Error.Unauthorized);
            }

            var cart = await _unitOfWork.GetRepository<Cart>()
                .Entity
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.AccountId == userResult.Value.Id);

            if (cart == null || !cart.CartItems.Any())
            {
                return Result<PaymentLinkResponse>.Fail("EMPTY_CART", "Cart is empty.");
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Quantity <= 0)
                {
                    return Result<PaymentLinkResponse>.Fail("INVALID_QUANTITY", "Cart item quantity must be greater than zero.");
                }

                if (item.ProductVariant.StockQuantity < item.Quantity)
                {
                    return Result<PaymentLinkResponse>.Fail("OUT_OF_STOCK", $"{item.ProductVariant.VariantName} does not have enough stock.");
                }
            }

            var payOsClient = CreatePayOsClient();
            if (payOsClient == null)
            {
                return Result<PaymentLinkResponse>.Fail("PAYOS_CONFIG_MISSING", "PayOS configuration is missing.");
            }

            var returnUrl = ResolveUrl(request.ReturnUrl, "PayOS:ReturnUrl");
            var cancelUrl = ResolveUrl(request.CancelUrl, "PayOS:CancelUrl");
            if (string.IsNullOrWhiteSpace(returnUrl) || string.IsNullOrWhiteSpace(cancelUrl))
            {
                return Result<PaymentLinkResponse>.Fail("PAYOS_URL_MISSING", "PayOS return url and cancel url are required.");
            }

            var orderCode = GenerateOrderCode();
            var order = new Order
            {
                AccountId = userResult.Value.Id,
                ReceiverName = request.ReceiverName.Trim(),
                ReceiverPhone = request.ReceiverPhone.Trim(),
                ShippingAddress = request.ShippingAddress.Trim(),
                PaymentMethod = PaymentMethod.Online,
                PaymentStatus = PaymentStatus.Pending,
                Status = OrderStatus.Pending,
                PaymentCode = orderCode.ToString(),
                TotalAmount = cart.CartItems.Sum(x => x.ProductVariant.Price * x.Quantity),
                CreatedAt = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(x => new OrderItem
                {
                    ProductVariantId = x.ProductVariantId,
                    Quantity = x.Quantity,
                    UnitPrice = x.ProductVariant.Price
                }).ToList()
            };



            try
            {
                var paymentRequest = new PayOS.Models.V2.PaymentRequests.CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Amount = decimal.ToInt32(decimal.Floor(order.TotalAmount)),
                    Description = $"Order {orderCode}",
                    ReturnUrl = AppendOrderCode(returnUrl, orderCode),
                    CancelUrl = AppendOrderCode(cancelUrl, orderCode)
                };

                var paymentLink = await payOsClient.PaymentRequests.CreateAsync(paymentRequest);
                order.PaymentUrl = paymentLink.CheckoutUrl;
                await _unitOfWork.GetRepository<Order>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return Result<PaymentLinkResponse>.Success(new PaymentLinkResponse
                {
                    OrderId = order.Id,
                    PaymentCode = order.PaymentCode,
                    CheckoutUrl = order.PaymentUrl,
                    Amount = order.TotalAmount,
                    Message = "Payment link created successfully."
                });
            }
            catch (PayOSException ex)
            {
                _logger.LogError(
                    ex,
                    "PayOS failed to create payment link for order code {OrderCode}. ErrorType: {ErrorType}. Message: {Message}",
                    orderCode,
                    ex.GetType().Name,
                    ex.Message);
                order.PaymentStatus = PaymentStatus.Failed;
                await _unitOfWork.SaveChangesAsync();
                return Result<PaymentLinkResponse>.Fail("PAYOS_CREATE_FAILED", "Could not create payment link.");
            }
        }

        public async Task<Result> HandlePayOsWebhookAsync(PayOSWebhookRequest request)
        {
            var payOsClient = CreatePayOsClient();
            if (payOsClient == null)
            {
                return Result.Fail(new Error("PAYOS_CONFIG_MISSING", "PayOS configuration is missing."));
            }

            WebhookData webhookData;
            try
            {
                webhookData = await payOsClient.Webhooks.VerifyAsync(ToPayOsWebhook(request));
            }
            catch (PayOSException ex)
            {
                _logger.LogWarning(ex, "PayOS webhook verification failed for order code {OrderCode}", request.Data.OrderCode);
                return Result.Fail(new Error("PAYOS_WEBHOOK_INVALID", "Invalid PayOS webhook."));
            }

            var orderCode = webhookData.OrderCode.ToString();
            var order = await _unitOfWork.GetRepository<Order>()
                .Entity
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.PaymentCode == orderCode);

            if (order == null)
            {
                return Result.Fail(Error.NotFound);
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                return Result.Success();
            }

            if (webhookData.Code != "00")
            {
                order.PaymentStatus = PaymentStatus.Failed;
                await _unitOfWork.SaveChangesAsync();
                return Result.Success();
            }

            if (order.TotalAmount != webhookData.Amount)
            {
                return Result.Fail(new Error("PAYMENT_AMOUNT_MISMATCH", "Payment amount does not match order amount."));
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.ProductVariant.StockQuantity < item.Quantity)
                    {
                        await _unitOfWork.RollBackAsync();
                        return Result.Fail(new Error("OUT_OF_STOCK", $"{item.ProductVariant.VariantName} does not have enough stock."));
                    }

                    item.ProductVariant.StockQuantity -= item.Quantity;
                    item.ProductVariant.UpdatedAt = DateTime.UtcNow;
                }

                order.PaymentStatus = PaymentStatus.Paid;
                order.Status = OrderStatus.Confirmed;

                var cart = await _unitOfWork.GetRepository<Cart>()
                    .Entity
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.AccountId == order.AccountId);

                if (cart != null && cart.CartItems.Any())
                {
                    await _unitOfWork.GetRepository<CartItem>().DeleteRangeAsync(cart.CartItems);
                }

                await _unitOfWork.CommitTransactionAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not complete paid order {OrderId}", order.Id);
                await _unitOfWork.RollBackAsync();
                return Result.Fail(new Error("PAYMENT_CONFIRM_FAILED", "Could not confirm payment."));
            }
        }

        public async Task<Result> CancelPaymentAsync(long orderCode)
        {
            var userResult = await _userService.GetUserLoginsAsync();
            if (userResult.Value == null)
            {
                return Result.Fail(Error.Unauthorized);
            }

            var order = await _unitOfWork.GetRepository<Order>()
                .FindAsync(o => o.PaymentCode == orderCode.ToString()
                    && o.AccountId == userResult.Value.Id
                    && o.PaymentStatus == PaymentStatus.Pending);

            if (order == null)
            {
                return Result.Fail(Error.NotFound);
            }

            var payOsClient = CreatePayOsClient();
            if (payOsClient == null)
            {
                return Result.Fail(new Error("PAYOS_CONFIG_MISSING", "PayOS configuration is missing."));
            }

            try
            {
                await payOsClient.PaymentRequests.CancelAsync(orderCode, "Customer cancelled payment");
            }
            catch (PayOSException ex)
            {
                _logger.LogWarning(ex, "PayOS cancel failed for order code {OrderCode}", orderCode);
            }

            order.PaymentStatus = PaymentStatus.Failed;
            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        private PayOSClient? CreatePayOsClient()
        {
            var clientId = _configuration["PayOS:ClientId"];
            var apiKey = _configuration["PayOS:ApiKey"];
            var checksumKey = _configuration["PayOS:ChecksumKey"];

            if (string.IsNullOrWhiteSpace(clientId)
                || string.IsNullOrWhiteSpace(apiKey)
                || string.IsNullOrWhiteSpace(checksumKey))
            {
                return null;
            }

            return new PayOSClient(clientId, apiKey, checksumKey);
        }

        private string? ResolveUrl(string? requestUrl, string configKey)
        {
            return !string.IsNullOrWhiteSpace(requestUrl)
                ? requestUrl
                : _configuration[configKey];
        }

        private static string AppendOrderCode(string url, long orderCode)
        {
            return url.Contains("{orderCode}", StringComparison.OrdinalIgnoreCase)
                ? url.Replace("{orderCode}", orderCode.ToString(), StringComparison.OrdinalIgnoreCase)
                : url;
        }

        private static long GenerateOrderCode()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var randomPart = Random.Shared.Next(100, 999);

            return checked(timestamp * 1000 + randomPart);
        }

        private static Webhook ToPayOsWebhook(PayOSWebhookRequest request)
        {
            return new Webhook
            {
                Code = request.Code,
                Description = request.Desc,
                Success = request.Success,
                Signature = request.Signature,
                Data = new WebhookData
                {
                    OrderCode = request.Data.OrderCode,
                    Amount = request.Data.Amount,
                    Description = request.Data.Description,
                    AccountNumber = request.Data.AccountNumber,
                    Reference = request.Data.Reference,
                    TransactionDateTime = request.Data.TransactionDateTime,
                    Currency = request.Data.Currency,
                    PaymentLinkId = request.Data.PaymentLinkId,
                    Code = request.Data.Code,
                    Description2 = request.Data.Desc,
                    CounterAccountBankId = request.Data.CounterAccountBankId,
                    CounterAccountBankName = request.Data.CounterAccountBankName,
                    CounterAccountName = request.Data.CounterAccountName,
                    CounterAccountNumber = request.Data.CounterAccountNumber,
                    VirtualAccountName = request.Data.VirtualAccountName,
                    VirtualAccountNumber = request.Data.VirtualAccountNumber,
                },
            };
        }
    }
}
