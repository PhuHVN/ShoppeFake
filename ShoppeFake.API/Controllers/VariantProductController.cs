using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

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
        public async Task<IActionResult> GetAll(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _variantProductService.GetAllVariantsAsync(pageIndex, pageSize);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse<BasePaginatedList<VariantResponse>>.OkResponse(result.Value, "Get All Variants", "200"));
            }
            return BadRequest(ApiResponse<BasePaginatedList<VariantResponse>>.BadRequestResponse("Failed to get variants"));
        }
        [HttpGet("{id}")]
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
        public async Task<IActionResult> Update(int id, [FromBody] VariantRequest request)
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
