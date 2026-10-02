using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.DTOs;
using ShoppeFake.Application.DTOs.ImgDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using Swashbuckle.AspNetCore.Annotations;

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
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Upload a product image", description: "Uploads a product image from form data and returns the stored image information.")]
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
        [SwaggerOperation(summary: "Public - Get all product images", description: "Retrieves a paginated list of all product images.")]
        public async Task<IActionResult> ListProductImages([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productImageService.ListProductImagesAsync(pageIndex, pageSize);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<BasePaginatedList<ImageResponse>>.OkResponse(result.Value, "Images retrieved successfully", "200"));
        }
        [HttpPut("variant/{variantId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Update product image by variant", description: "Updates the image for a specific product variant.")]
        public async Task<IActionResult> UpdateProductImage(int variantId, [FromForm] ImageDtos imageDtos)
        {
            var result = await _productImageService.UpdateProductImageAsync(variantId, imageDtos);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(null, "Image updated successfully", "200"));
        }

        [HttpDelete("variant/{variantId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(summary: "Admin - Delete product images by variant", description: "Deletes all images associated with a specific product variant.")]
        public async Task<IActionResult> DeleteProductImagesByVariant(int variantId)
        {
            var result = await _productImageService.DeleteProductImagesByVariantAsync(variantId);
            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<string>.BadRequestResponse(result.Error.Message));
            }
            return Ok(ApiResponse<string>.OkResponse(null, "Images deleted successfully", "200"));
        }

    }

}
