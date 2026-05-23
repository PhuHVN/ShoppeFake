using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.CategoryDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.CreateCategoryAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<CategoryResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<CategoryResponse>.OkResponse(result.Value, "Category created successfully.", "200"));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<CategoryResponse>.NotFoundResponse(result.Error.Message));
            }
            return Ok(ApiResponse<CategoryResponse>.OkResponse(result.Value, "Category retrieved successfully.", "200"));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _categoryService.GetAllCategoriesAsync(pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<BasePaginatedList<CategoryResponse>>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<CategoryResponse>>.OkResponse(result.Value, "Categories retrieved successfully.", "200"));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse<CategoryResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<CategoryResponse>.OkResponse(result.Value, "Category updated successfully.", "200"));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(null, "Category deleted successfully.", "200"));
        }
    }
}