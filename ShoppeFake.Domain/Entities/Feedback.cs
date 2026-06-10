using ShoppeFake.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Domain.Entities
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        //nagivation
        public string AccountId { get; set; } = string.Empty;
        public Account Account { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;


    }
}
