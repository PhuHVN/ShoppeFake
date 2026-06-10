using ShoppeFake.Application.DTOs.AttributeDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IAttributeService
    {
        Task<Result<AttributeResponse>> GetAttributeByIdAsync(int id);
        Task<Result<BasePaginatedList<AttributeResponse>>> GetAllAttributesAsync(int pageIndex, int pageSize);
        Task<Result<AttributeResponse>> CreateAttributeAsync(AttributeRequest request);
        Task<Result<AttributeResponse>> UpdateAttributeAsync(int id, AttributeRequest request);
        Task DeleteAttributeAsync(int id);
    }
}
