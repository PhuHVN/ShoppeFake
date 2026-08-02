using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Domain.Entities
{
    public class Order
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

        public Account Account { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
