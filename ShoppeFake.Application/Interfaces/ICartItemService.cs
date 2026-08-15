using ShoppeFake.Application.DTOs.CartItemDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface ICartItemService
    {
        Task<Result<CartItemResponse>> GetCartItemByIdAsync(int id);
        Task<Result<BasePaginatedList<CartItemResponse>>> GetCartItemsByAccountAsync(int pageIndex, int pageSize);
        Task<Result<CartItemResponse>> CreateCartItemAsync(CartItemRequest request);
        Task<Result<CartItemResponse>> UpdateCartItemAsync(int id, int quantity);
        Task<Result> DeleteCartItemAsync(int id);
        Task<Result<string>> DeleteItemCartByItemIdAsync(int itemId);
    }
}
