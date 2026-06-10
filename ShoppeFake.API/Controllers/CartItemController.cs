using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.CartItemDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/cart-items")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new cart item", Description = "Adds a new item to the shopping cart.")]
        public async Task<IActionResult> CreateCartItem([FromBody] CartItemRequest request)
        {
            var result = await _cartItemService.CreateCartItemAsync(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<CartItemResponse>.OkResponse(result.Value, "Cart item created successfully.", "200"));
            }
            return BadRequest(result.Error);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get cart items for a specific account", Description = "Retrieves the list of cart items for a specific account.")]
        public async Task<IActionResult> GetCartItemsByAccount([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _cartItemService.GetCartItemsByAccountAsync(pageIndex, pageSize);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<BasePaginatedList<CartItemResponse>>.OkResponse(result.Value, "Cart items retrieved successfully.", "200"));
            }
            return BadRequest(result.Error);
        }
        [HttpPut("{id}/{quantity}")]
        [SwaggerOperation(Summary = "Update cart item quantity", Description = "Updates the quantity of a specific cart item.")]
        public async Task<IActionResult> UpdateCartItemQuantity(int id, int quantity)
        {
            var result = await _cartItemService.UpdateCartItemAsync(id, quantity);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<CartItemResponse>.OkResponse(result.Value, "Cart item quantity updated successfully.", "200"));
            }
            return BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a cart item", Description = "Removes a specific item from the shopping cart.")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var result = await _cartItemService.DeleteCartItemAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.Error);
        }
    }
}
