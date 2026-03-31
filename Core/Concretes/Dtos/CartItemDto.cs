namespace Core.Concretes.Dtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountedPrice   { get; set; }
        public int Quantity { get; set; }
    }
}
