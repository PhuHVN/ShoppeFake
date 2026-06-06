using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.ImgDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;

        public ImageController(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProductImage([FromForm] ImageDtos imageDtos)
        {
            var result = await _productImageService.UploadProductImageAsync(imageDtos);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(result.Value, "Image uploaded successfully", "201"));
        }
        [HttpGet]
        public async Task<IActionResult> ListProductImages([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productImageService.ListProductImagesAsync(pageIndex, pageSize);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<ImageResponse>>.OkResponse(result.Value, "Images retrieved successfully", "200"));
        }
    }
}