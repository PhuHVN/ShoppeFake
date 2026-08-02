using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.OrderItemDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(ApiResponse<GetOrderReponse>.OkResponse(order.Value, "Order found", "200"));
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
            return Ok(ApiResponse<BasePaginatedList<GetOrderReponse>>.OkResponse(orders.Value, "Orders found", "200"));
        }
    }
}
