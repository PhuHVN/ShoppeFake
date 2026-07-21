using Microsoft.Extensions.Logging;
using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Infrastructure.Implemention
{
    public sealed class ChatApiClient : IChatApiClient
    {
        private const string Endpoint = "/api/v1/chat/conversations/messages";
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatApiClient> _logger;

        public ChatApiClient(HttpClient httpClient, ILogger<ChatApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ChatApiResponse?> SendMessageAsync(SendChatMessageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(Endpoint,request,cancellationToken);

                var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>(cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Web1 Chat API returned {StatusCode}. Message: {Message}",
                        response.StatusCode,
                        result?.Message);

                    return result;
                }

                return result;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(
                    exception,
                    "Cannot connect to Web1 Chat API.");

                return null;
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    exception,
                    "Web1 Chat API request timed out.");

                return null;
            }
        }
    }
}
