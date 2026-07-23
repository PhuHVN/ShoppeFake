using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiService
    {
        Task<ChatApiResponse?> SendV1Async(string message, CancellationToken cancellationToken = default);
        Task<ChatApiResponse?> SendV2Async(string conversationId, string message, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(int pageIndex,int pageSize, CancellationToken cancellationToken = default);
        Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(string conversationId,string? lastCursor,int limit, CancellationToken cancellationToken = default);
    }
}
