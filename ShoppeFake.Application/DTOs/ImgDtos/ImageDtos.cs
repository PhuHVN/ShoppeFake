using Microsoft.AspNetCore.Http;
using ShoppeFake.Domain.Entities;
using ShoppeFake.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.ImgDtos
{
    public class ImageDtos
    {
        public IFormFile Image { get; set; } = null!;
        public int ProductId { get; set; }
        public int VariantId { get; set; }
    }
}
