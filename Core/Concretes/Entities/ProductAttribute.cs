using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    /// <summary>
    /// Ürünün özniteliği/özelliği
    /// Örnek: Renk, Boyut, Malzeme vb.
    /// </summary>
    public class ProductAttribute : BaseEntity  // ✅ FIX: BaseEntity'den türet
    {
        /// <summary>
        /// Özniteliğin adı (örn: "Renk", "Boyut")
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Özniteliğin değeri (örn: "Kırmızı", "L")
        /// </summary>
        public string Value { get; set; } = null!;

        //Foreign key
        /// <summary>
        /// Bu özniteliğin ait olduğu ürünün ID'si
        /// </summary>
        public int ProductId { get; set; }

        // Navigation property
        /// <summary>
        /// Bu özniteliğin ait olduğu ürün nesnesi
        /// </summary>
        public virtual Product? Product { get; set; }  // ✅ FIX: Products -> Product
    }
}