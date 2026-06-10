using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.AttributeDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/attributes")]
    [ApiController]
    public class AttributeController : ControllerBase
    {
        private readonly IAttributeService _attributeService;
        public AttributeController(IAttributeService attributeService)
        {
            _attributeService = attributeService;
        }

        [HttpGet]
        [SwaggerOperation(summary: "Get all attributes", description: "Retrieves a paginated list of all product attributes.")]
        public async Task<IActionResult> GetAllAttributes(int pageIndex = 1, int pageSize = 10)
        {
            var result = await _attributeService.GetAllAttributesAsync(pageIndex, pageSize);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<AttributeResponse>>.OkResponse(result.Value, "Attributes retrieved successfully", "200"));
        }
        [HttpGet("{id}")]
        [SwaggerOperation(summary: "Get attribute by ID", description: "Retrieves the details of a product attribute using its unique identifier.")]
        public async Task<IActionResult> GetAttributeById(int id)
        {
            var result = await _attributeService.GetAttributeByIdAsync(id);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<AttributeResponse>.OkResponse(result.Value, "Attribute retrieved successfully", "200"));
        }
        [HttpPost]
        [SwaggerOperation(summary: "Create a new attribute", description: "Creates a new product attribute with the provided details.")]
        public async Task<IActionResult> CreateAttribute([FromBody] AttributeRequest request)
        {
            var result = await _attributeService.CreateAttributeAsync(request);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<AttributeResponse>.OkResponse(result.Value, "Attribute created successfully", "201"));
        }

    }
}
