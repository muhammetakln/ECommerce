namespace Core.Concretes.Dtos
{
    public class CartDto
    {
        public string CustomerId { get; set; } = null!;
        public IEnumerable<CartItemDto> Items { get; set; } = [];
    }
    // ❌ CartItemDto ALINAN KALMADI!
}