using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Domain.Entities
{
    public class Attribute
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsFilterable { get; set; } = false;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //navigation
        //1- M with AttributeValue
        public ICollection<AttributeValue> AttributeValues { get; set; } = null!;
        //1- M with VariantAttributeValue
        public ICollection<VariantAttributeValue> VariantAttributeValues { get; set; } = null!;
    }
}
