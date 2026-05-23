using ShoppeFake.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Domain.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //navigation
        public ICollection<Product> Products { get; set; } = null!;
    }
}
