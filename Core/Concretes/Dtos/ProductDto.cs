using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Concretes.DTOs
{
    public class ProductListItemDto
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

        public string BrandName { get; set; } = null!;
        public string SubCategoryName { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    /// <summary>
    /// Ürün yorumu (review) için DTO
    /// </summary>
    public class ProductReviewDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int Vote { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}