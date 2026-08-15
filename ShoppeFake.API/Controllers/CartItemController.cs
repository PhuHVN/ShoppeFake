using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.CartItemDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/cart-items")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpPost]
        [SwaggerOperation(summary: "Customer - Create a new cart item", description: "Adds a new item to the shopping cart.")]
        public async Task<IActionResult> CreateCartItem(AddToCartSource addToCartSource, [FromBody] CartItemRequest request)
        {
            request.Source = addToCartSource;
            var result = await _cartItemService.CreateCartItemAsync(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<CartItemResponse>.OkResponse(result.Value, "Cart item created successfully.", "200"));
            }
            return BadRequest(result.Error);
        }

        [HttpGet]
        [SwaggerOperation(summary: "Customer - Get own cart items", description: "Retrieves the list of cart items for a specific account.")]
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
        [SwaggerOperation(summary: "Customer - Update own cart item quantity", description: "Updates the quantity of a specific cart item.")]
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
        [SwaggerOperation(summary: "Customer - Delete specific item in cart by Id", description: "Removes a specific item from the shopping cart.")]
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


