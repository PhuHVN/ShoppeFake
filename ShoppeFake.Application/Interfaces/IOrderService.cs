using ShoppeFake.Application.DTOs.OrderItemDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<GetOrderReponse>> GetOrderByIdAsync(int orderId);
        Task<Result<BasePaginatedList<GetOrderReponse>>> GetOrdersAsync(int pageNumber, int pageSize);
    }
}
