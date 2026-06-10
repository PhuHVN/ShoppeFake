using ShoppeFake.Application.DTOs.ValueDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IAttributeValueService
    {
        Task<Result<ValueResponse>> CreateValueAsync(ValueRequest request);
        Task<Result<ValueResponse>> UpdateValueAsync(int id, ValueRequest request);
        Task<Result<bool>> DeleteValueAsync(int id);
        Task<Result<BasePaginatedList<ValueResponse>>> GetAllValuesAsync(int pageIndex, int pageSize);
        Task<Result<ValueResponse>> GetValueByIdAsync(int id);
    }
}
