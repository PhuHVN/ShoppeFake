using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImgageUrl { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //navigation
        public int ProductId { get; set; }
        public Product Product { get; set; } =null!;
        public int VariantId { get; set; }
        public ProductVariant Variant { get; set; } = null!;
    }
}
