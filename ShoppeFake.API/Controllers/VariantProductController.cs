using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.ExcelDtos;
using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/variants")]
    [ApiController]
    public class VariantProductController : ControllerBase
    {
        private readonly IVariantService _variantProductService;
        public VariantProductController(IVariantService variantProductService)
        {
            _variantProductService = variantProductService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Create a new product variant", description: "Creates a new product variant and associates it with the provided attribute value IDs.")]
        public async Task<IActionResult> Add([FromQuery] IList<int> valueIds, [FromBody] VariantRequest request)
        {
            var result = await _variantProductService.CreateVariantAsync(valueIds, request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<VariantResponse>.OkResponse(result.Value, "Variant created successfully", "200"));
            }
            return BadRequest(ApiResponse<string>.BadRequestResponse("Failed to add variant"));
        }


        [HttpGet]
        [SwaggerOperation(summary: "Public - Get all product variants", description: "Retrieves a paginated list of all product variants.")]
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10 , string? orderBy = null)
        {
            var result = await _variantProductService.GetAllVariantsAsync(pageIndex, pageSize, orderBy);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<BasePaginatedList<VariantResponse>>.OkResponse(result.Value, "Get All Variants", "200"));
            }
            return BadRequest(ApiResponse<BasePaginatedList<VariantResponse>>.BadRequestResponse("Failed to get variants"));
        }

        [HttpGet("export")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Export product variants", description: "Retrieves a list of all product variants for export purposes.")]
        public async Task<IActionResult> GetAll2()
        {
            var result = await _variantProductService.GetAllToExportAsync();
            return Ok(ApiResponse<IList<ProductVariantExportDto>>.OkResponse(result, "Get All Variants", "200"));
        }


        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Public - Get product variant by ID", description: "Retrieves the details of a product variant using its unique identifier.")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _variantProductService.GetVariantByIdAsync(id);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<VariantResponse>.OkResponse(result.Value, "Get Variant by Id", "200"));
            }
            return BadRequest(ApiResponse<VariantResponse>.BadRequestResponse("Failed to get variant"));
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Delete a product variant", description: "Deletes a product variant using its unique identifier.")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _variantProductService.DeleteVariantAsync(id);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(ApiResponse<string>.BadRequestResponse("Failed to delete variant"));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Update a product variant", description: "Updates the details of an existing product variant.")]
        public async Task<IActionResult> Update(int id, [FromBody] VariantUpdateRequest request)
        {
            var result = await _variantProductService.UpdateVariantAsync(id, request);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<VariantResponse>.OkResponse(result.Value, "Variant updated successfully", "200"));
            }
            return BadRequest(ApiResponse<string>.BadRequestResponse("Failed to update variant"));
        }
    }

}
