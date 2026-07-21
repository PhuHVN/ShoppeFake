using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.ChatApiDtos
{
    public sealed class ChatApiOptions
    {
        public const string SectionName = "ChatApi";

        public string BaseUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }
}
