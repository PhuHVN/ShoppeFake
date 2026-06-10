using Microsoft.AspNetCore.Http;

namespace ShoppeFake.Application.DTOs.ImgDtos
{
    public class ImageDtos
    {
        public IFormFile Image { get; set; } = null!;
        public int ProductId { get; set; }
        public int VariantId { get; set; }
    }
}
