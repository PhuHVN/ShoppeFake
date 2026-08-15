using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayOS;
using PayOS.Exceptions;

using PayOS.Models.Webhooks;
using ShoppeFake.Application.DTOs.ChatApiDtos;
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
        private readonly IChatApiService _chatService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IUserService userService,
            IConfiguration configuration,
            IChatApiService chatService,
            ILogger<PaymentService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _configuration = configuration;
            _chatService = chatService;
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
                //=======================================
                //get cart items that are from chat source and have conversationId
                var cartItems = await _unitOfWork.GetRepository<CartItem>()
                    .FilterByAsync(ci => ci.CartId == cart.Id && ci.Source == AddToCartSource.Chat && !string.IsNullOrEmpty(ci.ConversationId));
                var conversationGroups = cartItems.GroupBy(x => x.ConversationId);
                // Callback to chat service if conversationId is provided
                foreach (var conversationGroup in conversationGroups)
                {
                    try
                    {
                        var conversationId = conversationGroup.Key;

                        var productVariantIds = conversationGroup
                            .Select(x => x.ProductVariantId)
                            .ToHashSet();

                        var orderItems = order.OrderItems
                            .Where(x => productVariantIds.Contains(x.ProductVariantId))
                            .ToList();

                        if (!orderItems.Any())
                            continue;

                        var orderEvents = new OrdersRequest
                        {
                            ExternalOrderId = order.Id,
                            Amount = orderItems.Sum(x => x.UnitPrice * x.Quantity),
                            Status = order.Status.ToString(),
                            Products = orderItems.Select(item => new OrderProductItemRequest
                            {
                                ExternalProductId = item.ProductVariantId.ToString(),
                                ProductName = item.ProductVariant?.VariantName ?? string.Empty,
                                Quantity = item.Quantity,
                                Price = item.UnitPrice
                            }).ToList()
                        };

                        await _chatService.OrderEventAsync(conversationId!, orderEvents);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "Failed to send order event to chat service for conversation {ConversationId}",
                            conversationGroup.Key);
                    }
                }

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

        public async Task<bool> HandlePayOsWebhookAsync(PayOSWebhookRequest request)
        {
            // Validate request signature
            if (request.Data == null || string.IsNullOrWhiteSpace(request.Signature))
            {
                _logger.LogWarning("PayOS webhook rejected because required payload fields are missing.");
                return true; // Business logic error: invalid signature
            }

            // Get PayOS client
            var payOsClient = CreatePayOsClient();
            if (payOsClient == null)
            {
                _logger.LogError("PayOS client is not configured. Cannot process webhook.");
                return false; // System error: service dependency unavailable
            }

            // Verify webhook signature
            WebhookData webhookData;
            try
            {
                webhookData = await payOsClient.Webhooks.VerifyAsync(ToPayOsWebhook(request));
                _logger.LogInformation("PayOS webhook verified successfully");
            }
            catch (PayOSException ex)
            {
                _logger.LogWarning(ex, "PayOS webhook verification failed.");
                return true; // Business logic error: invalid signature
            }

            var orderCode = webhookData.OrderCode.ToString();

            // Get order from database
            var order = null as Order;
            try
            {
                order = await _unitOfWork.GetRepository<Order>()
               .Entity
               .Include(o => o.OrderItems)
               .ThenInclude(oi => oi.ProductVariant)
               .FirstOrDefaultAsync(o => o.PaymentCode == orderCode);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error when retrieving order. PaymentCode={PaymentCode}", orderCode);
                return false; // Database error
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when retrieving order. PaymentCode={PaymentCode}", orderCode);
                return false; // System error
            }

            // Order not found - business logic issue
            if (order == null)
            {
                _logger.LogWarning("Order not found. PaymentCode={PaymentCode}", orderCode);
                return true; // Business logic error: order doesn't exist
            }

            // Payment already processed
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogWarning("Paid order received again. PaymentCode={PaymentCode}", orderCode);
                return true; // Business logic: already processed
            }

            // Check payment code from PayOS
            if (webhookData.Code != "00")
            {
                _logger.LogWarning("Payment failed. PaymentCode={PaymentCode}, Code={Code}", orderCode, webhookData.Code);
                try
                {
                    order.PaymentStatus = PaymentStatus.Failed;
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error when updating failed order. PaymentCode={PaymentCode}", orderCode);
                    await _unitOfWork.RollBackAsync();
                    return false; // Database error: transaction commit failed
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error when updating failed order. PaymentCode={PaymentCode}", orderCode);
                    await _unitOfWork.RollBackAsync();
                    return false; // System error
                }
                return true; // Business logic: payment failed
            }

            // Check amount mismatch
            if (order.TotalAmount != webhookData.Amount)
            {
                _logger.LogWarning("Amount mismatch. PaymentCode={PaymentCode}, Expected={Expected}, Actual={Actual}",
                    orderCode, order.TotalAmount, webhookData.Amount);
                try
                {
                    order.PaymentStatus = PaymentStatus.Failed;
                    order.Status = OrderStatus.Cancelled;
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Order marked as cancelled due to amount mismatch. OrderId={OrderId}", order.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to mark order as cancelled for amount mismatch. OrderId={OrderId}", order.Id);
                }
                return true; // Business logic error: amount mismatch
            }

            // Process successful payment
            try
            {
                var processStatus = await ProcessPaidOrderAsync(order, webhookData);
                if (processStatus == WebhookProcessStatus.Success)
                {
                    return true; // Successfully processed
                }
                else
                {
                    // Business logic issues (out of stock, etc.)
                    return true;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error when processing paid order. OrderId={OrderId}", order.Id);
                await _unitOfWork.RollBackAsync();
                return false; // Database error: transaction commit failed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when processing paid order. OrderId={OrderId}", order.Id);
                await _unitOfWork.RollBackAsync();
                return false; // System error
            }
        }
        private async Task<WebhookProcessStatus> ProcessPaidOrderAsync(Order order, WebhookData webhookData)
        {
            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogWarning("Paid order received. PaymentCode={PaymentCode}", order.PaymentCode);
                return WebhookProcessStatus.Ignore;
            }

            // Validate stock availability for all items
            foreach (var item in order.OrderItems)
            {
                if (item.ProductVariant.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Out of stock for product variant {ProductVariantId}. PaymentCode={PaymentCode}", item.ProductVariantId, order.PaymentCode);
                    return WebhookProcessStatus.OutOfStock;
                }
            }

            // Reduce stock for all items (atomic transaction)
            try
            {
                foreach (var item in order.OrderItems)
                {
                    item.ProductVariant.StockQuantity -= item.Quantity;
                }

                order.PaymentStatus = PaymentStatus.Paid;
                order.Status = OrderStatus.Confirmed;
                await _unitOfWork.GetRepository<Order>().UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync(); // Atomic update with stock changes
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Stock update failed due to concurrency. OrderId={OrderId}", order.Id);
                await _unitOfWork.RollBackAsync();
                return WebhookProcessStatus.OutOfStock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database error when reducing stock. OrderId={OrderId}", order.Id);
                await _unitOfWork.RollBackAsync();
                return WebhookProcessStatus.Retry;
            }

            // Clear cart only after payment is confirmed and stock is reduced successfully
            try
            {
                var cart = await _unitOfWork.GetRepository<Cart>()
                    .FindAsync(c => c.AccountId == order.AccountId);

                if (cart != null)
                {
                    var cartItems = await _unitOfWork.GetRepository<CartItem>().FilterByAsync(ci => ci.CartId == cart.Id);
                    foreach (var item in cartItems)
                    {
                        await _unitOfWork.GetRepository<CartItem>().DeleteAsync(item);
                    }
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Cart cleared for order {OrderId}", order.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear cart for order {OrderId}. Cart may have duplicate items.", order.Id);
                // Don't block order confirmation if cart clear fails
            }

            // Notify chat service about the order status update (fire and forget - don't block if it fails)
            try
            {
                await _chatService.UpdateStatusEventAsync(order.Id.ToString(), order.Status.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to notify chat service for order {OrderId}. Order still confirmed in database.", order.Id);               
            }

            return WebhookProcessStatus.Success;
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
                _logger.LogWarning(ex, "PayOS cancel failed for order code {OrderCode}. Order cancelled locally but PayOS sync may be delayed.", orderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error cancelling order in PayOS. OrderCode={OrderCode}", orderCode);
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
                Code = request.Code!,
                Description = request.Desc!,
                Success = request.Success,
                Signature = request.Signature!,
                Data = new WebhookData
                {
                    OrderCode = request.Data.OrderCode,
                    Amount = request.Data.Amount,
                    Description = request.Data.Description!,
                    AccountNumber = request.Data.AccountNumber!,
                    Reference = request.Data.Reference!,
                    TransactionDateTime = request.Data.TransactionDateTime!,
                    Currency = request.Data.Currency!,
                    PaymentLinkId = request.Data.PaymentLinkId!,
                    Code = request.Data.Code!,
                    Description2 = request.Data.Desc!,
                    CounterAccountBankId = request.Data.CounterAccountBankId!,
                    CounterAccountBankName = request.Data.CounterAccountBankName!,
                    CounterAccountName = request.Data.CounterAccountName!,
                    CounterAccountNumber = request.Data.CounterAccountNumber!,
                    VirtualAccountName = request.Data.VirtualAccountName!,
                    VirtualAccountNumber = request.Data.VirtualAccountNumber!,
                },
            };
        }
    }
}
