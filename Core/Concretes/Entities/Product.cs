using Core.Abstracts.Bases;
using System.ComponentModel.Design;

namespace Core.Concretes.Entities
{

    /// <summary>
    /// Product entity representing items available for sale in the e-commerce store.
    /// Müşteri tarafı için basitleştirilmiş versiyon.
    /// </summary>
    /// <remarks>
    /// Neden Product entity kullanılır?
    /// 1. Ürün Temsili: Müşterilerin satın alabileceği ürünleri temsil eder
    /// 2. Veri Kalıcılığı: Ürün bilgilerini veritabanına eşler
    /// 3. Katalog Yönetimi: Fiyatlandırma ve ürün detayları içerir
    /// 4. İlişki Yönetimi: Kategoriler, markalar ve siparişlere bağlanır
    /// 5. Arama & Filtreleme: Müşterilerin ürün bulması için özellikleri sağlar
    /// </remarks>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Ürün adı (müşterilerin göreceği isim).
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Ürün açıklaması.
        /// Ürün detay sayfasında gösterilir.
        /// </summary>
        public string? Description { get; set; }


        public decimal DiscountRate { get; set; } = 0;

        /// <summary>
        /// Marka kimliği (Foreign Key).
        /// Ürünün hangi markaya ait olduğunu gösterir.
        /// </summary>

        public decimal Price { get; set; }





        public int StockQuantity { get; set; } = 0;
        // foreign keys
        public int SubCategoryId { get; set; }
        public int BrandId { get; set; }
        // navigation properties
        public virtual SubCategory? SubCategory { get; set; }
        public virtual Brand? Brand { get; set; }

        public virtual ICollection<ProductAttribute> Attributes { get; set; } = [];
        public virtual ICollection<ProductImage> Images { get; set; } = [];
        public virtual ICollection<CartItem> CartItems { get; set; } = [];
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = [];
        public virtual ICollection<Order> Orders { get; set; } = [];

    }

}
