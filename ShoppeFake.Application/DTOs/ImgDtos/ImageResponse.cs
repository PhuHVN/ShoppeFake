using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.ImgDtos
{
    public class ImageResponse
    {
        public string ImageUrl { get; set; } = null!;
        public int ProductId { get; set; }
        public int VariantId { get; set; }
    }
}
