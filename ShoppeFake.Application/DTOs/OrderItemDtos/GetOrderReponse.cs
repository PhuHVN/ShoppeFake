using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.DTOs.OrderItemDtos
{
    public class GetOrderReponse
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string? PaymentCode { get; set; }
        public string? PaymentUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductVariantName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public AddToCartSource AddToCartSource { get; set; } = AddToCartSource.Product;
        public string? ConversationId { get; set; } 
    }
}
