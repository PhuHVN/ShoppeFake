using System.Text.Json.Serialization;

namespace ShoppeFake.Application.DTOs.ChatApiDtos
{
    public class SendRequest
    {
        public string Message { get; set; } = string.Empty;
    }
    public class OrdersRequest {

        public int ExternalOrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public List<OrderProductItemRequest> Products { get; set; } = new();
    }
}
