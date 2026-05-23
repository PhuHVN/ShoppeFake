using ShoppeFake.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //navigation
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<ProductVariant> ProductVariants { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = null!;
    }
}
