using AutoMapper;
using ShoppeFake.Application.DTOs.ImgDtos;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using ShoppeFake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Services
{
    public class ImageService : IProductImageService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ImageService(ICloudinaryService cloudinaryService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<BasePaginatedList<ImageResponse>>> ListProductImagesAsync(int pageIndex, int pageSize)
        {
            var query = _unitOfWork.GetRepository<ProductImage>().Entity;
            var rs = await _unitOfWork.GetRepository<ProductImage>().GetPagging(query, pageIndex, pageSize);
            return Result<BasePaginatedList<ImageResponse>>.Success(_mapper.Map<BasePaginatedList<ImageResponse>>(rs));
        }

        public async Task<Result<string>> UploadProductImageAsync(ImageDtos imageDtos)
        {
            if(imageDtos == null || imageDtos.Image == null)
            {
                return Result<string>.Fail("400", "Image file is required.");
            }
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(imageDtos.ProductId);
            if(product == null)
            {
                return Result<string>.Fail("404", $"Product not found.");
            }
            var variant = await _unitOfWork.GetRepository<ProductVariant>().GetByIdAsync(imageDtos.VariantId);
            if(variant == null)
            {
                return Result<string>.Fail("404", $"Variant not found.");
            }
            try
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(imageDtos.Image);
                if (uploadResult == null)
                {
                    return Result<string>.Fail("500", $"Image upload failed: {uploadResult}");
                }
                var productImage = new ProductImage
                {
                    ProductId = imageDtos.ProductId,
                    VariantId = imageDtos.VariantId,
                    ImageUrl = uploadResult
                };
                await _unitOfWork.GetRepository<ProductImage>().AddAsync(productImage);
                await _unitOfWork.SaveChangesAsync();
                return Result<string>.Success("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail("500", $"Image upload failed: {ex.Message}");
            }


        }
    }
}
