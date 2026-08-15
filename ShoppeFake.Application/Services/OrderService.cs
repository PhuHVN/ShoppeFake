using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.OrderItemDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<Result<GetOrderReponse>> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.GetRepository<Order>()
                .FindAsync(
                    o => o.Id == orderId,
                    query => query
                        .AsNoTracking()
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant));

            return Result<GetOrderReponse>.Success(_mapper.Map<GetOrderReponse>(order));
        }

        public async Task<Result<BasePaginatedList<GetOrderReponse>>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            var user = await _userService.GetUserLoginsAsync();
            if (user == null || user.Value == null)
            {
                return Result<BasePaginatedList<GetOrderReponse>>.Fail("404", "User not found");
            }
            var query = _unitOfWork.GetRepository<Order>()
                .Entity
                .AsNoTracking()
                .Where(o => o.AccountId == user.Value.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(cr => cr.CartItems)
                .OrderByDescending(o => o.CreatedAt);

            var orders = await _unitOfWork.GetRepository<Order>().GetPagging(query, pageNumber, pageSize);

            return Result<BasePaginatedList<GetOrderReponse>>.Success(_mapper.Map<BasePaginatedList<GetOrderReponse>>(orders));
        }
    }
}
