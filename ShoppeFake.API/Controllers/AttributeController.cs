using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.AttributeDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

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