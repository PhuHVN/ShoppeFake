using ShoppeFake.Application.DTOs.PaymentDtos;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<PaymentLinkResponse>> CreatePaymentLinkFromCartAsync(CheckoutPaymentRequest request);
        Task<bool> HandlePayOsWebhookAsync(PayOSWebhookRequest request);
        Task<Result> CancelPaymentAsync(long orderCode);
    }
}
