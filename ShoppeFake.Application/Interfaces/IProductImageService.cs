using ShoppeFake.Application.DTOs.ImgDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;

namespace ShoppeFake.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<Result<string>> UploadProductImageAsync(ImageDtos imageDtos);
        Task<Result<BasePaginatedList<ImageResponse>>> ListProductImagesAsync(int pageIndex, int pageSize);
    }
}
