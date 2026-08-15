using ShoppeFake.Domain.Enums;
using System.Text.Json.Serialization;

namespace ShoppeFake.Application.DTOs.CartItemDtos
{
    public class CartItemRequest
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string? ConversationId { get; set; } = string.Empty;
        [JsonIgnore]
        public AddToCartSource Source { get; set; } = AddToCartSource.Product;
    }
}
