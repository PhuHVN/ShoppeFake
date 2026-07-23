using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiClient
    {
        Task<ChatApiResponse?> SendMessageV1Async(SendChatMessageClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse?> SendMessageV2Async(string? conversationId, SendChatMessageClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(GetChatHistoryClientRequest request, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(CustomerGetChatHistoryClientRequest request, CancellationToken cancellationToken = default);
    }
}
