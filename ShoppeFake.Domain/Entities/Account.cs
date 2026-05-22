using ShoppeFake.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Domain.Entities
{
    public class Account
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public RoleEnum Role { get; set; } = RoleEnum.Customer;
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        

    }
}
