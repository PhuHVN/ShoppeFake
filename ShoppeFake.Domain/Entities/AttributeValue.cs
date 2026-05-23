using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Domain.Entities
{
    public class AttributeValue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ValueText { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //navigation
        //1 - M with VariantAttributeValue
        public ICollection<VariantAttributeValue> VariantAttributeValues { get; set; } = null!;
        //M - 1 with Attribute
        public int AttributeId { get; set; }
        public Domain.Entities.Attribute Attribute { get; set; } = null!;


    }
}
