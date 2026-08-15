using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.CartItemDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public CartItemService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }


        public async Task<Result<CartItemResponse>> CreateCartItemAsync(CartItemRequest request)
        {
            if (request.Quantity <= 0)
            {
                return Result<CartItemResponse>.Fail("InvalidQuantity", "Quantity must be greater than zero.");
            }
            var cartResult = await GetOrCreateCartByAccountLoginAsync();
            if (!cartResult.IsSuccess)
            {
                return Result<CartItemResponse>.Fail(cartResult.Error);
            }
            var cart = cartResult.Value!;
            var productVariant = await _unitOfWork.GetRepository<ProductVariant>().FindAsync(x => x.Id == request.ProductVariantId);
            if (productVariant == null)
            {
                return Result<CartItemResponse>.Fail("NotFound", "Product variant not found.");
            }

            if (request.Quantity > productVariant.StockQuantity)
            {
                return Result<CartItemResponse>.Fail("OutOfStock", $"Only {productVariant.StockQuantity} items left in stock.");
            }

            var existingCartItem = await _unitOfWork.GetRepository<CartItem>().FindAsync(x => x.CartId == cart.Id && x.ProductVariantId == request.ProductVariantId);
            if (existingCartItem != null)
            {
                if (existingCartItem.Quantity + request.Quantity > productVariant.StockQuantity)
                {
                    return Result<CartItemResponse>.Fail("OutOfStock", $"Adding {request.Quantity} items exceeds available stock. Only {productVariant.StockQuantity - existingCartItem.Quantity} more items can be added.");
                }
                existingCartItem.Quantity += request.Quantity;
                existingCartItem.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.GetRepository<CartItem>().UpdateAsync(existingCartItem);
                await _unitOfWork.SaveChangesAsync();
                var response1 = _mapper.Map<CartItemResponse>(existingCartItem);
                return Result<CartItemResponse>.Success(response1);
            }
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductVariantId = request.ProductVariantId, 
                Source = request.Source,
                Quantity = request.Quantity
            };
            if(request.ConversationId != null || !string.IsNullOrEmpty(request.ConversationId))
            {
                cartItem.ConversationId = request.ConversationId;
            }
            await _unitOfWork.GetRepository<CartItem>().AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<CartItemResponse>(cartItem);
            return Result<CartItemResponse>.Success(response);
        }
        private async Task<Result<Cart>> GetOrCreateCartByAccountLoginAsync()
        {
            var user = await _userService.GetUserLoginsAsync();
            if (user.Value == null)
            {
                return Result<Cart>.Fail("Unauthorized", "User must be logged in to add items to cart.");
            }
            var cart = await _unitOfWork.GetRepository<Cart>().FindAsync(x => x.AccountId == user.Value.Id);
            if (cart == null)
            {
                cart = new Cart
                {
                    AccountId = user.Value.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.GetRepository<Cart>().AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }
            return Result<Cart>.Success(cart);
        }

        public async Task<Result> DeleteCartItemAsync(int id)
        {
            var user = await _userService.GetUserLoginsAsync();
            if (user.Value == null)
            {
                return Result.Fail(Error.Unauthorized);
            }
            var cartItem = await _unitOfWork.GetRepository<CartItem>().FindAsync(x => x.Id == id && x.Cart.AccountId == user.Value.Id);
            if (cartItem == null)
            {
                return Result.Fail(Error.NotFound);
            }
            await _unitOfWork.GetRepository<CartItem>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public Task<Result<CartItemResponse>> GetCartItemByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<BasePaginatedList<CartItemResponse>>> GetCartItemsByAccountAsync(int pageIndex, int pageSize)
        {
            var cartResult = await GetOrCreateCartByAccountLoginAsync();
            if (!cartResult.IsSuccess)
            {
                return Result<BasePaginatedList<CartItemResponse>>.Fail(cartResult.Error);
            }
            var cart = cartResult.Value!;
            var query = _unitOfWork.GetRepository<CartItem>().Entity.AsNoTracking().Include(x => x.ProductVariant).ThenInclude(pv => pv.Product).Where(x => x.CartId == cart.Id);
            var rs = await _unitOfWork.GetRepository<CartItem>().GetPagging(query, pageIndex, pageSize);
            var response = _mapper.Map<BasePaginatedList<CartItemResponse>>(rs);
            return Result<BasePaginatedList<CartItemResponse>>.Success(response);


        }
        public async Task<Result<string>> DeleteItemCartByItemIdAsync(int itemId)
        {
            var user = await _userService.GetUserLoginsAsync();
            if (user.Value == null)
            {
                return Result<string>.Fail("Unauthorized", "User must be logged in to delete cart items.");
            }
            var cartItem = await _unitOfWork.GetRepository<CartItem>().FindAsync(x => x.Id == itemId && x.Cart.AccountId == user.Value.Id);
            if (cartItem == null)
            {
                return Result<string>.Fail("NotFound", "Cart item not found.");
            }
            await _unitOfWork.GetRepository<CartItem>().DeleteAsync(itemId);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Cart item deleted successfully.");
        }
        public async Task<Result<CartItemResponse>> UpdateCartItemAsync(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return Result<CartItemResponse>.Fail("InvalidQuantity", "Quantity must be greater than zero.");
            }
            var user = await _userService.GetUserLoginsAsync();
            if (user.Value == null)
            {
                return Result<CartItemResponse>.Fail(Error.Unauthorized);
            }
            var cartItem = await _unitOfWork.GetRepository<CartItem>().FindAsync(x => x.Id == id && x.Cart.AccountId == user.Value.Id);
            if (cartItem == null)
            {
                return Result<CartItemResponse>.Fail(Error.NotFound);
            }
            cartItem.Quantity = quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.GetRepository<CartItem>().UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<CartItemResponse>(cartItem);
            return Result<CartItemResponse>.Success(response);
        }
    }
}
