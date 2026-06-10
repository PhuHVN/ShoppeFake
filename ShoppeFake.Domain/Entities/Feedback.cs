using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //nagivation
        public string AccountId { get; set; } = string.Empty;
        public Account Account { get; set; } = null!;
        public int ProductId { get; set; } 
        public Product Product { get; set; } = null!;


    }
}
