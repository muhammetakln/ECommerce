namespace Core.Concretes.DTOs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }

        /// <summary>
        /// İndirim oranı (%)
        /// Formül: ((Price - DiscountedPrice) / Price) * 100
        /// Örnek: Price=100, DiscountedPrice=80 => DiscountRate=20%
        /// </summary>
        public decimal DiscountRate
        {
            get
            {
                if (Price == 0) return 0;
                return ((Price - DiscountedPrice) / Price) * 100;
            }
        }

        public string Description { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string SubCategoryName { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = null!;

        public string CategoryName { get; set; } = null!;
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public int Stock { get; set; }
        public bool Active { get; set; }
        public List<ProductReviewDto> Reviews { get; set; } = new List<ProductReviewDto>();
    }
}