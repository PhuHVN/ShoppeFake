using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.ProductDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<ProductResponse>.OkResponse(result.Value, "Product created successfully", "200"));
            }
            return BadRequest(ApiResponse<ProductResponse>.BadRequestResponse("Failed to create product"));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _productService.GetAllProductsAsync(pageIndex, pageSize);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<BasePaginatedList<ProductResponse>>.OkResponse(result.Value, "Products retrieved successfully", "200"));
            }
            return NotFound(ApiResponse<BasePaginatedList<ProductResponse>>.NotFoundResponse("No products found"));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<ProductResponse>.OkResponse(result.Value, "Product retrieved successfully", "200"));
            }
            return NotFound(ApiResponse<ProductResponse>.NotFoundResponse("Product not found"));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductRequest request)
        {
            var result = await _productService.UpdateProductAsync(id, request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<ProductResponse>.OkResponse(result.Value, "Product updated successfully", "200"));
            }
            return BadRequest(ApiResponse<ProductResponse>.BadRequestResponse("Failed to update product"));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(ApiResponse<string>.NotFoundResponse("Product not found"));
        }
    }
}