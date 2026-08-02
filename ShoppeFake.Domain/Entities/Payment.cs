using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? Provider { get; set; }              // PayOS, VNPay, MoMo, COD
        public string? ProviderTransactionId { get; set; } // mã giao dịch từ cổng thanh toán
        public string? PaymentUrl { get; set; }            // link checkout nếu có
        public string? OrderCode { get; set; }             // mã order gửi sang provider

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public string? FailureReason { get; set; }

        public Order Order { get; set; } = null!;
    }
}
