namespace ShoppeFake.Application.DTOs.PaymentDtos
{
    public class CheckoutPaymentRequest
    {
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty; 
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
