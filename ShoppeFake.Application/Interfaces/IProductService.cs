using ShoppeFake.Application.DTOs.ProductDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<ProductResponse>> GetProductByIdAsync(int id);
        Task<Result<BasePaginatedList<ProductResponse>>> GetAllProductsAsync(int pageIndex, int pageSize);
        Task<Result<ProductResponse>> CreateProductAsync(ProductRequest request);
        Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest request);
        Task<Result> DeleteProductAsync(int id);
    }
}
