using Core.Abstracts.Bases;

namespace Core.Concretes.Entities
{
    /// <summary>
    /// Sepetteki ürün öğesi.
    /// 
    /// Neden BaseEntity'den türetilmesi gerekli?
    /// 1. CreatedAt ve UpdatedAt alanları ile data consistency
    /// 2. Soft delete (Deleted alanı) desteği
    /// 3. Diğer tüm entity'lerle tutarlılık
    /// 4. Audit trail (kim ne zaman ekledi/güncelledi)
    /// </summary>
    public class CartItem : BaseEntity  // ✅ FIX: BaseEntity'den türet
    {
        /// <summary>
        /// Sepetteki bu ürünün miktarı
        /// </summary>
        public int Quantity { get; set; }

        // Foreign Keys
        /// <summary>
        /// Bu ürün öğesinin ait olduğu sepet ID'si
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// Bu ürün öğesinin ürün ID'si
        /// </summary>
        public int ProductId { get; set; }

        // Navigation Properties
        /// <summary>
        /// Bu ürün öğesinin ait olduğu sepet nesnesi
        /// </summary>
        public virtual Cart? Cart { get; set; }

        /// <summary>
        /// Bu ürün öğesinin ürün nesnesi (Product detayları)
        /// </summary>
        public virtual Product? Product { get; set; }
    }
}