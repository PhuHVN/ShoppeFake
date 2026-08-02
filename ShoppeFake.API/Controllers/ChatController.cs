using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs.ChatApiDtos;
using ShoppeFake.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/chat")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class ChatController : ControllerBase
    {
        private readonly IChatApiService _chatApiService;

        public ChatController(IChatApiService chatApiService)
        {
            _chatApiService = chatApiService;
        }

        [HttpPost("messages")]
        [SwaggerOperation(summary: "Customer - Send a chat message", description: "Sends a chat message as the logged-in customer.")]
        public async Task<IActionResult> SendV1Message([FromBody] SendRequest request, CancellationToken cancellationToken)
        {
            var result = await _chatApiService.SendV1Async(request.Message, cancellationToken);
            if (result == null)
            {
                return BadRequest("Failed to send message.");
            }
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpPost("{conversationId}/messages")]
        [SwaggerOperation(summary: "Customer - Send a chat message with conversationId", description: "Sends a chat message as the logged-in customer.")]
        public async Task<IActionResult> SendvV2Message(string conversationId, [FromBody] SendRequest request, CancellationToken cancellationToken)
        {
            var result = await _chatApiService.SendV2Async(conversationId, request.Message, cancellationToken);
            if (result == null)
            {
                return BadRequest("Failed to send message.");
            }
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpGet]
        [SwaggerOperation(summary: "Customer - Get a history chat ")]
        public async Task<IActionResult> GetHistoryChat(int pageIndex = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _chatApiService.CustomerGetChatHistoryAsync(pageIndex, pageSize, cancellationToken);
            if (result == null)
            {
                return BadRequest("Failed to send message.");
            }
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
        [HttpGet("{conversationId}/messages")]
        [SwaggerOperation(summary: "Customer - Get chat messages for a specific conversation")]
        public async Task<IActionResult> GetCursorChatHistory(string conversationId, string? lastCursor, int limit = 20, CancellationToken cancellationToken = default)
        {
            var result = await _chatApiService.GetCursorChatHistoryAsync(conversationId, lastCursor, limit, cancellationToken);
            if (result == null)
            {
                return BadRequest("Failed to retrieve chat history.");
            }
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }
            return Ok(result);
        }
    }
}
