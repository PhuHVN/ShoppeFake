using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.ValueDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/attribute-values")]
    [ApiController]
    public class AttributeValueController : ControllerBase
    {
        private readonly IAttributeValueService _attributeValueService;

        public AttributeValueController(IAttributeValueService attributeValueService)
        {
            _attributeValueService = attributeValueService;
        }
        [HttpPost]
        [SwaggerOperation(summary: "Create a new attribute value", description: "Creates a new value for a product attribute with the provided details.")]
        public async Task<IActionResult> CreateAttributeValue(ValueRequest request)
        {
            var result = await _attributeValueService.CreateValueAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<ValueResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<ValueResponse>.OkResponse(result.Value, "Value created successfully.", "200"));
        }
        [HttpGet]
        [SwaggerOperation(summary: "Get all attribute values", description: "Retrieves a paginated list of all product attribute values.")]
        public async Task<IActionResult> GetAllAttributeValues(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _attributeValueService.GetAllValuesAsync(pageIndex, pageSize);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<BasePaginatedList<ValueResponse>>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<ValueResponse>>.OkResponse(result.Value, "Values retrieved successfully.", "200"));
        }
        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Get attribute value by ID", description: "Retrieves the details of an attribute value using its unique identifier.")]
        public async Task<IActionResult> GetAttributeValueById(int id)
        {
            var result = await _attributeValueService.GetValueByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<ValueResponse>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<ValueResponse>.OkResponse(result.Value, "Value retrieved successfully.", "200"));
        }
    }
}
