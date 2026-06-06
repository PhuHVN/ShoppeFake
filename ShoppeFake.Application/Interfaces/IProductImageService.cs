using ShoppeFake.Application.DTOs.ImgDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<Result<string>> UploadProductImageAsync(ImageDtos imageDtos);
        Task<Result<BasePaginatedList <ImageResponse>>> ListProductImagesAsync( int pageIndex, int pageSize);
    }
}
