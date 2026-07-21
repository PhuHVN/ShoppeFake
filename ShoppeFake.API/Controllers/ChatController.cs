using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
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

        [HttpPost("send")]
        [SwaggerOperation(summary: "Customer - Send a chat message", description: "Sends a chat message as the logged-in customer.")]
        public async Task<IActionResult> SendMessage([FromBody] SendRequest request, CancellationToken cancellationToken)
        {
            var result = await _chatApiService.SendAsync(request.Message, cancellationToken);
            if (result == null)
            {
                return BadRequest(ApiResponse<ChatApiResponse>.BadRequestResponse("Failed to send message."));
            }
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<ChatApiResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<ChatApiResponse?>.OkResponse(result.Value, "Message sent successfully.", "200"));
        }
    }
}
