namespace ShoppeFake.Application.DTOs.CartItemDtos
{
    public class CartItemResponse
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }



    }
}
