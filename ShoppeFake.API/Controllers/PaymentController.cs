using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.PaymentDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] CheckoutPaymentRequest request)
        {
            var result = await _paymentService.CreatePaymentLinkFromCartAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest($"Error creating payment link : {result.Error.Message}");
            }
            return Ok(ApiResponse<PaymentLinkResponse>.OkResponse(result.Value, "Payment link created successfully", "200"));
        }
        [HttpPost("callback")]
        public async Task<IActionResult> HandlePaymentCallback([FromBody] PayOsWebhookRequest request)
        {
            var result = await _paymentService.HandlePayOsWebhookAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest($"Error handling payment callback : {result.Error.Message}");
            }
            return Ok(result);
        }
        

    }
}
