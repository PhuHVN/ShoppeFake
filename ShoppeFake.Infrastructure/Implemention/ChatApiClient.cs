using Microsoft.Extensions.Logging;
using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace ShoppeFake.Infrastructure.Implemention
{
    public sealed class ChatApiClient : IChatApiClient
    {
        private const string Endpoint = "/api/v1/chat/conversations";
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatApiClient> _logger;

        public ChatApiClient(HttpClient httpClient, ILogger<ChatApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }



        public async Task<ChatApiResponse?> SendMessageV1Async(SendChatMessageClientRequest request, CancellationToken cancellationToken = default)
        {
            var urlMessage = Endpoint + "/messages";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(urlMessage, request, cancellationToken);

                var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>(cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "SmartChatBot API returned {StatusCode}. Message: {Message}",
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
                    "Cannot connect to SmartChatBot API.");

                return null;
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    exception,
                    "SmartChatBot API request timed out.");

                return null;
            }
        }
        public async Task<ChatApiResponse?> SendMessageV2Async(string? conversationId, SendChatMessageClientRequest request, CancellationToken cancellationToken = default)
        {
            var url = Endpoint + $"/{conversationId}/messages";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);

                var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>(cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "SmartChatBot API returned {StatusCode}. Message: {Message}",
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
                    "Cannot connect to SmartChatBot API.");

                return null;
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    exception,
                    "SmartChatBot API request timed out.");

                return null;
            }
        }
        public async Task<ChatApiResponse<PagingResponse<ConversationResponse>>?> CustomerGetChatHistoryAsync(CustomerGetChatHistoryClientRequest request, CancellationToken cancellationToken = default)
        {

            var url = Endpoint + $"?externalCustomerId={request.ExternalCustomerId}&pageIndex={request.PageIndex}&pageSize={request.PageSize}";
            try
            {
                using var response = await _httpClient.GetAsync(url, cancellationToken);

                var content = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning(
                        "SmartChatBot returned empty response. Status: {StatusCode}",
                        response.StatusCode);

                    return null;
                }


                return JsonSerializer.Deserialize<
                    ChatApiResponse<PagingResponse<ConversationResponse>>>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException exception)
            {
                _logger.LogError(
                    exception,
                    "Cannot deserialize conversation response");

                return null;
            }
        }




        public async Task<ChatApiResponse<PagingResponse<ConversationMessageResponse>>?> GetCursorChatHistoryAsync(GetChatHistoryClientRequest request, CancellationToken cancellationToken = default)
        {
            var url = string.Empty;
            if (!string.IsNullOrEmpty(request.LastCursor))
            {
                url = Endpoint + $"/{request.ConversationId}/messages?externalCustomerId={request.ExternalCustomerId}&lastCursor={request.LastCursor}&limit={request.Limit}";
            }
            else
            {
                url = Endpoint + $"/{request.ConversationId}/messages?externalCustomerId={request.ExternalCustomerId}&limit={request.Limit}";
            }
            try
            {

                using var response = await _httpClient.GetAsync(url, cancellationToken);

                var content = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.LogWarning(
                        "SmartChatBot returned empty response. Status: {StatusCode}",
                        response.StatusCode);

                    return null;
                }


                return JsonSerializer.Deserialize<
                    ChatApiResponse<PagingResponse<ConversationMessageResponse>>>(
                    content,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch (JsonException exception)
            {
                _logger.LogError(
                    exception,
                    "Cannot deserialize conversation response");

                return null;
            }
        }

        public async Task<bool> OrderEventAsync(OrderEventClientRequest request,CancellationToken cancellationToken = default)
        {
            var url = Endpoint + $"{request.ConversationId}/orders";
            try
            {
                using var response = await _httpClient.PostAsJsonAsync(
                    url,
                    request,
                    cancellationToken);

                var result = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "SmartChatBot API returned {StatusCode}. Message: {Message}",
                        response.StatusCode,
                        result);

                    return false;
                }

                return true;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Cannot connect to SmartChatBot API.");

                return false;
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    exception,
                    "SmartChatBot API request timed out.");

                return false;
            }
        }
        public async Task<bool> UpdateStatusEvent(string orderId,string status, CancellationToken cancellationToken = default)
        {
            var url = Endpoint + $"/orders/{orderId}/status";
            try
            {
                using var response = await _httpClient.PatchAsJsonAsync(url, new { status }, cancellationToken);

                var result = await response.Content.ReadFromJsonAsync<ChatApiResponse>(cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "SmartChatBot API returned {StatusCode}. Message: {Message}",
                        response.StatusCode,
                        result?.Message);

                    return false;
                }

                return true;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(
                    exception,
                    "Cannot connect to SmartChatBot API.");

                return false;
            }
            catch (TaskCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogError(
                    exception,
                    "SmartChatBot API request timed out.");

                return false;
            }
        }
    }
}



