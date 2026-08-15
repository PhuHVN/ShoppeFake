using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiClient
    {
        Task<ChatApiResponse?> SendMessageV1Async(SendChatMessageClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse?> SendMessageV2Async(string? conversationId, SendChatMessageClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(GetChatHistoryClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(CustomerGetChatHistoryClientRequest request, CancellationToken cancellationToken = default);
        Task<bool> OrderEventAsync(OrderEventClientRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusEvent(string orderId, EnumStatusClient status, CancellationToken cancellationToken = default);
    }
}
