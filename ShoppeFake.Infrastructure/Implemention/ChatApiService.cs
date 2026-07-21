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
        public async Task<Result<ChatApiResponse?>> SendAsync(string message, CancellationToken cancellationToken = default)
        {
            var userLogin = await _userService.GetUserLoginsAsync();
            if (userLogin is null || userLogin.Value is null)
            {
                return Result<ChatApiResponse?>.Fail("404", "User not logged in");

            }
            var chatResponse = await _chatApiClient.SendMessageAsync(
                new SendChatMessageRequest
                {
                    Message = message,
                    ExternalCustomerId = userLogin.Value.FullName.ToString()
                },
                cancellationToken);
            return Result<ChatApiResponse?>.Success(chatResponse);
        }
    }
}
