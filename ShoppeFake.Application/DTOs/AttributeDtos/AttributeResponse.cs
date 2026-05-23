using ShoppeFake.Application.DTOs.ValueDtos;
using ShoppeFake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.AttributeDtos
{
    public class AttributeResponse
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; }  = string.Empty;
        public IList<ValueResponseV1> AttributeValues { get; set; } = new List<ValueResponseV1>();
    }
    public class ValueResponseV1
    {
        public string ValueText { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}
