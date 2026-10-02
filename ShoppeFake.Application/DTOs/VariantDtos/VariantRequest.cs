using Microsoft.AspNetCore.Http;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.DTOs.VariantDtos
{
    public class VariantRequest
    {
        public int ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int WeightGrams { get; set; }
    }
    public class VariantUpdateRequest
    {
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int WeightGrams { get; set; }
        public List<int> VariantAttributeValuesIds { get; set; } = new ();
    }
}
