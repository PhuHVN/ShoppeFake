using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Infrastructure.Implemention
{
    public sealed class ChatApiService : IChatApiService
    {
        private readonly IChatApiClient _chatApiClient;
        private readonly IUserService _userService;

        public ChatApiService(IChatApiClient chatApiClient, IUserService userService)
        {
            _chatApiClient = chatApiClient;
            _userService = userService;
        }

        public async Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var userLogin = await _userService.GetUserLoginsAsync();
            if (userLogin is null || userLogin.Value is null)
            {
                return null;

            }
            var chatResponse = await _chatApiClient.CustomerGetChatHistoryAsync(
                new CustomerGetChatHistoryClientRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    ExternalCustomerId = userLogin.Value.Id.ToString()
                },
                cancellationToken);
            return chatResponse;
        }

        public async Task<ChatApiResponse?> SendV1Async(string message, CancellationToken cancellationToken = default)
        {
            var userLogin = await _userService.GetUserLoginsAsync();
            if (userLogin is null || userLogin.Value is null)
            {
                return null;

            }
            var chatResponse = await _chatApiClient.SendMessageV1Async(
                new SendChatMessageClientRequest
                {
                    Message = message,
                    ExternalCustomerId = userLogin.Value.Id.ToString()
                },
                cancellationToken);
            return chatResponse;
        }

        public async Task<ChatApiResponse?> SendV2Async(string conversationId, string message, CancellationToken cancellationToken = default)
        {
            var userLogin = await _userService.GetUserLoginsAsync();
            if (userLogin is null || userLogin.Value is null)
            {
                return null;

            }
            var chatResponse = await _chatApiClient.SendMessageV2Async(
                conversationId,
                new SendChatMessageClientRequest
                {
                    Message = message,
                    ExternalCustomerId = userLogin.Value.Id.ToString()
                },
                cancellationToken);
            return chatResponse;
        }
        public async Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(string conversationId, string lastCursor, int limit, CancellationToken cancellationToken = default)
        {
            var userLogin = await _userService.GetUserLoginsAsync();
            if (userLogin is null || userLogin.Value is null)
            {
                return null;
            }
            var chatResponse = await _chatApiClient.GetCursorChatHistoryAsync(
                new GetChatHistoryClientRequest
                {
                    ConversationId = conversationId,
                    ExternalCustomerId = userLogin.Value.Id.ToString(),
                    LastCursor = lastCursor,
                    Limit = limit
                },
                cancellationToken);
            return chatResponse;
        }
    }
}
