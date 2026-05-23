using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IVariantService
    {
        Task<Result<VariantResponse>> GetVariantByIdAsync(int id);
        Task<Result<BasePaginatedList<VariantResponse>>> GetAllVariantsAsync(int pageIndex, int pageSize);
        Task<Result<VariantResponse>> CreateVariantAsync(IList<int> valueIds, VariantRequest request);
        Task<Result<VariantResponse>> UpdateVariantAsync(int id, VariantRequest request);
        Task<Result> DeleteVariantAsync(int id);
    }
}
