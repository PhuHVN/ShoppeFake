using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //navigation
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int VariantId { get; set; }
        public ProductVariant Variant { get; set; } = null!;
    }
}
