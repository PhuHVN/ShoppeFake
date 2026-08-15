using System.Text.Json;

namespace ShoppeFake.Application.DTOs.ChatApiDtos
{
    public class ChatApiResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string MessageCode { get; set; } = string.Empty;

        public Web1ChatData? Data { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }
    }
    public sealed class Web1ChatData
    {
        public string ConversationId { get; set; } = string.Empty;

        public string ConversationTitle { get; set; } = string.Empty;

        public string MessageResponse { get; set; } = string.Empty;
    }



    //ListHistory
    public sealed class ChatApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string MessageCode { get; set; } = string.Empty;

        public T? Data { get; set; }

        public JsonElement? Errors { get; set; }
    }
    public sealed class PagingResponse<T>
    {
        public List<T> Items { get; set; } = [];

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int PageSize { get; set; }
    }
    public sealed class ConversationResponse
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTimeOffset LastMessageAt { get; set; }

        public DateTimeOffset CreateAt { get; set; }
    }
    //CursorPage
    public class ConversationMessageResponse
    {
        public string? Id { get; set; }

        public required string Content { get; set; }

        public string SenderType { get; set; } = string.Empty;

        public string ContentType { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
