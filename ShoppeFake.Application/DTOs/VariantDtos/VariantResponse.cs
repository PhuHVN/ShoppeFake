using ShoppeFake.Application.DTOs.AttributeDtos;
using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.VariantDtos
{
    public class VariantResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int WeightGrams { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public IList<VariantAttributeValueResponse> VariantAttributes { get; set; } = new List<VariantAttributeValueResponse>();

    }
    public class VariantAttributeValueResponse
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string AttributeCode { get; set; } = string.Empty;
        public IList<ValueResponseV1> Values { get; set; } = new List<ValueResponseV1>();

    }
}
