
using ShoppeFake.Domain.Enums;

namespace ShoppeFake.Application.DTOs.AccountDtos
{
    public class AccountResponse
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
    }
}
