using System.Text.Json.Serialization;

namespace ShoppeFake.Application.DTOs.ChatApiDtos
{
    public class SendChatMessageClientRequest
    {
        public string Message { get; set; } = string.Empty;

        public string? ExternalCustomerId { get; set; }
    }
    public class GetChatHistoryClientRequest
    {
        public string ConversationId { get; set; } = string.Empty;

        public string ExternalCustomerId { get; set; } = string.Empty;

        public string? LastCursor { get; set; }

        public int Limit { get; set; } = 20;
    }
    public class CustomerGetChatHistoryClientRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string ExternalCustomerId { get; set; } = string.Empty;
    }
    public class OrderEventClientRequest
    {
        public string ConversationId { get; set; } = string.Empty;
        public string ExternalOrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public List<OrderProductItemRequest> Products { get; set; } = new();
    }

    public class OrderProductItemRequest
    {  
        public string ExternalProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;    
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
