namespace ShoppeFake.Application.DTOs.PaymentDtos
{
    public class PaymentLinkResponse
    {
        public int OrderId { get; set; }
        public string PaymentCode { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
