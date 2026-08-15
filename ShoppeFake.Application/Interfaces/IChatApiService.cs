using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiService
    {
        Task<ChatApiResponse?> SendV1Async(string message, CancellationToken cancellationToken = default);
        Task<ChatApiResponse?> SendV2Async(string conversationId, string message, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(string conversationId, string? lastCursor, int limit, CancellationToken cancellationToken = default);
        Task<bool> OrderEventAsync(string conversationId, OrdersRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusEventAsync( string orderId, EnumStatusClient status, CancellationToken cancellationToken = default);
    }
}
