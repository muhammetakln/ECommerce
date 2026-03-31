using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    /// <summary>
    /// Ürün resmi/görüntüsü
    /// </summary>
    public class ProductImage : BaseEntity  // ✅ FIX: BaseEntity'den türet
    {
        /// <summary>
        /// Resmin URL'i (dosya yolu)
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Bu resim ürünün kapak resmi mi?
        /// </summary>
        public bool IsCoverImage { get; set; } = false;

        //Foreign key
        /// <summary>
        /// Bu resmin ait olduğu ürünün ID'si
        /// </summary>
        public int ProductId { get; set; }

        // Navigation property
        /// <summary>
        /// Bu resmin ait olduğu ürün nesnesi
        /// </summary>
        public virtual Product? Product { get; set; }  // ✅ FIX: Products -> Product
    }
}