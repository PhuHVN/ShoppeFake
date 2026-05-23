using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Domain.Entities
{
    public class VariantAttributeValue
    {
        [Key]
        public int Id { get; set; }

        //navigation
        //M - 1 with ProductVariant
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        //M - 1 with Attribute
        public int AttributeId { get; set; }
        public Attribute Attribute { get; set; } = null!;
        //M - 1 with AttributeValue
        public int AttributeValueId { get; set; }
        public AttributeValue AttributeValue { get; set; } = null!;
    }
}
