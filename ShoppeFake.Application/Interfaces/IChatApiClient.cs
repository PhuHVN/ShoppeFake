using ShoppeFake.Application.DTOs.ChatApiDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IChatApiClient
    {
        Task<ChatApiResponse?> SendMessageAsync(SendChatMessageRequest request, CancellationToken cancellationToken = default);
    }
}
