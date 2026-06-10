namespace ShoppeFake.Application.DTOs.ImgDtos
{
    public class ImageResponse
    {
        public string ImageUrl { get; set; } = null!;
        public int ProductId { get; set; }
        public int VariantId { get; set; }
    }
}
