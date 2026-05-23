using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppeFake.Application.DTOs.ProductDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ProductResponse>> CreateProductAsync(ProductRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || request.CategoryId <= 0 || string.IsNullOrEmpty(request.Slug)
                || string.IsNullOrEmpty(request.Description))
            {
                return Result<ProductResponse>.Fail(Error.Invalid);
            }
            if (string.IsNullOrEmpty(request.Brand))
            {
                request.Brand = "Unknown";
            }
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return Result<ProductResponse>.Fail(Error.NotFound);
            }
            var product = new Product
            {
                Name = request.Name,
                CategoryId = request.CategoryId,
                Slug = request.Slug,
                Description = request.Description,
                Brand = request.Brand,
                Status = Domain.Enums.StatusEnum.Active,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.GetRepository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<ProductResponse>(product);
            return Result<ProductResponse>.Success(response);
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == id, include: x => x.Include(x => x.Category));
            if (product == null)
            {
                return Result.Fail(Error.NotFound);
            }
            product.Status = Domain.Enums.StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<BasePaginatedList<ProductResponse>>> GetAllProductsAsync(int pageIndex, int pageSize)
        {
            var products = _unitOfWork.GetRepository<Product>().Entity.Include(x => x.Category).Where(x => x.Status == Domain.Enums.StatusEnum.Active);
            var rs = await _unitOfWork.GetRepository<Product>().GetPagging(products, pageIndex, pageSize);
            var response = _mapper.Map<BasePaginatedList<ProductResponse>>(rs);
            return Result<BasePaginatedList<ProductResponse>>.Success(response);
        }

        public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == id, include: x => x.Include(x => x.Category));
            if (product == null)
            {
                return Result<ProductResponse>.Fail(Error.NotFound);
            }
            return Result<ProductResponse>.Success(_mapper.Map<ProductResponse>(product));
        }

        public async Task<Result<ProductResponse>> UpdateProductAsync(int id, ProductRequest request)
        {
            var product = await _unitOfWork.GetRepository<Product>().FindAsync(x => x.Id == id, include: x => x.Include(x => x.Category));
            if (product == null)
            {
                return Result<ProductResponse>.Fail(Error.NotFound);
            }
            if (string.IsNullOrEmpty(request.Name) || request.CategoryId <= 0 || string.IsNullOrEmpty(request.Slug)
                || string.IsNullOrEmpty(request.Description))
            {
                return Result<ProductResponse>.Fail(Error.Invalid);
            }
            if (string.IsNullOrEmpty(request.Brand))
            {
                request.Brand = "Unknown";
            }
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                return Result<ProductResponse>.Fail(Error.NotFound);
            }
            product.Name = request.Name;
            product.Description = request.Description;
            product.Brand = request.Brand;
            product.Slug = request.Slug;
            await _unitOfWork.SaveChangesAsync();
            return Result<ProductResponse>.Success(_mapper.Map<ProductResponse>(product));
        }
    }
}
