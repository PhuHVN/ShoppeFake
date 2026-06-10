namespace ShoppeFake.Domain.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //navigation
        public Account Account { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = null!;
    }
}
