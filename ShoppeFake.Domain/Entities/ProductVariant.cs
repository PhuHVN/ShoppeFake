using ShoppeFake.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Domain.Entities
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int WeightGrams { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //navigation
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        //1 - M with VariantAttributeValue
        public ICollection<VariantAttributeValue> VariantAttributeValues { get; set; } = null!;
        //1 - M with ProductImage
        public ICollection<ProductImage> ProductImages { get; set; } = null!;
    }
}
