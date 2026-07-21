using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.DTOs.ChatApiDtos
{
    public class ChatApiResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string MessageCode { get; set; } = string.Empty;

        public Web1ChatData? Data { get; set; }

        public Dictionary<string, string>? errors { get; set; }
    }
    public sealed class Web1ChatData
    {
        public string ConversationId { get; set; } = string.Empty;

        public string ConversationTitle { get; set; } = string.Empty;

        public string MessageResponse { get; set; } = string.Empty;
    }
}
