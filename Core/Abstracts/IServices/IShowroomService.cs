using Core.Concretes.DTOs;
using Utils.Responses;

namespace Core.Abstracts.IServices
{
    /// <summary>
    /// Showroom (e-ticaret) sayfasında gösterilecek ürünler için servis interface'i
    /// Ürün listesi, detayları, düzenleme ve silme işlemlerini tanımlar
    /// </summary>
    public interface IShowroomService
    {
        /// <summary>
        /// Aktif ve silinmemiş tüm ürünleri getirir (list view için)
        /// </summary>
        Task<IResult<IEnumerable<ProductListItemDto>>> GetProductAsync();

        /// <summary>
        /// Belirtilen ID'ye sahip ürünün detay bilgilerini getirir
        /// </summary>
        Task<IResult<ProductDetailDto>> GetProductDetailAsync(int id);

        /// <summary>
        /// Ürünü günceller (Edit işlemi)
        /// Admin tarafından kullanılır
        /// </summary>
        Task<IResult> UpdateProductAsync(int id, ProductDetailDto dto);

        /// <summary>
        /// Ürünü siler (soft delete - Deleted flag'ı true yapılır)
        /// Admin tarafından kullanılır
        /// </summary>
        Task<IResult> DeleteProductAsync(int id);
    }
}