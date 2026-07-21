using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.FeedbackDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(summary: "Customer - Create a new feedback for a product", description: "Creates a new feedback for a product.")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackRequest request)
        {
            var result = await _feedbackService.CreateFeedbackAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<FeedbackResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<FeedbackResponse>.OkResponse(result.Value, "Feedback created successfully.", "200"));
        }
        [HttpGet("product/{productId}")]
        [SwaggerOperation(summary: "Public - Get feedbacks for a product", description: "Gets feedbacks for a specific product.")]
        public async Task<IActionResult> GetFeedbacksByProductId(int productId, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _feedbackService.GetFeedbacksByProductIdAsync(productId, pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<BasePaginatedList<FeedbackResponse>>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<FeedbackResponse>>.OkResponse(result.Value, "Feedbacks retrieved successfully.", "200"));
        }
    }
}
