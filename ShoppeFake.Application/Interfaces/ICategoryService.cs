using ShoppeFake.Application.DTOs.CategoryDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id);
        Task<Result<BasePaginatedList<CategoryResponse>>> GetAllCategoriesAsync(int pageIndex, int pageSize);
        Task<Result<CategoryResponse>> CreateCategoryAsync(CategoryRequest request);
        Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<Result> DeleteCategoryAsync(int id);
    }
}
