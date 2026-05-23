using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.DTOs.ProductDtos
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
