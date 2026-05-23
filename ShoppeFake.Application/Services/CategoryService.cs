using AutoMapper;
using ShoppeFake.Application.DTOs.CategoryDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<CategoryResponse>> CreateCategoryAsync(CategoryRequest request)
        {
            if (request.Name == null || request.Name.Trim() == "")
            {
                return Result<CategoryResponse>.Fail("InvalidData", "Category name is required.");
            }
            var existingCategory = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Name.ToLower() == request.Name.ToLower() || c.Status == StatusEnum.Inactive);
            if (existingCategory != null)
            {
                return Result<CategoryResponse>.Fail("DuplicateData", "A category with the same name already exists.");
            }
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                Status = StatusEnum.Active,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category));

        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Id == id && c.Status == StatusEnum.Active);
            if (category == null)
            {
                return Result.Fail(Error.NotFound);
            }
            category.Status = StatusEnum.Inactive;
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result<BasePaginatedList<CategoryResponse>>> GetAllCategoriesAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<Category>().Entity.Where(c => c.Status == StatusEnum.Active);

            var rs = await _unitOfWork.GetRepository<Category>().GetPagging(query, pageIndex, pageSize);
            return Result<BasePaginatedList<CategoryResponse>>.Success(_mapper.Map<BasePaginatedList<CategoryResponse>>(rs));
        }

        public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Id == id && c.Status == StatusEnum.Active);
            if (category == null)
            {
                return Result<CategoryResponse>.Fail("NotFound", "Category not found.");
            }
            return Result<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category));
        }

        public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var category = await _unitOfWork.GetRepository<Category>().FindAsync(c => c.Id == id && c.Status == StatusEnum.Active);
            if (category == null)
            {
                return Result<CategoryResponse>.Fail("NotFound", "Category not found.");
            }
            if (request.Name != null && request.Name.Trim() != "")
            {
                category.Name = request.Name;
            }
            if (request.Description != null)
            {
                category.Description = request.Description;
            }
            await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            return Result<CategoryResponse>.Success(_mapper.Map<CategoryResponse>(category));
        }
    }
}
