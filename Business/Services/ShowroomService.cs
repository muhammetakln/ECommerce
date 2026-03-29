using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.DTOs;
using Utils.Responses;

namespace Business.Services
{
    public class ShowroomService : IShowroomService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ShowroomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <summary>
        /// Aktif ve silinmemiş tüm ürünleri getirir
        /// </summary>
        public async Task<IResult<IEnumerable<ProductListItemDto>>> GetProductAsync()
        {
            var result = await unitOfWork.ProductRepository.FindManyAsync(
                x => x.Active && !x.Deleted,
                "SubCategory", "Brand", "Images", "ProductReviews"
            );

            if (!result.IsSuccess)
            {
                return Result<IEnumerable<ProductListItemDto>>.Failure(
                    result.Message ?? "Ürünler yüklenemedi",
                    result.StatusCode
                );
            }

            var products = mapper.Map<IEnumerable<ProductListItemDto>>(result.Data);
            return Result<IEnumerable<ProductListItemDto>>.Success(products, 200, "Ürünler yüklendi");
        }

        /// <summary>
        /// Belirtilen ID'ye sahip ürünün detay bilgilerini getirir
        /// </summary>
        public async Task<IResult<ProductDetailDto>> GetProductDetailAsync(int id)
        {
            try
            {
                var result = await unitOfWork.ProductRepository.FindFirstAsync(
                    x => x.Id == id && x.Active && !x.Deleted,
                    "SubCategory",
                    "SubCategory.Category",
                    "Brand",
                    "Images",
                    "ProductReviews",
                    "ProductReviews.Customer"
                );

                if (!result.IsSuccess)
                {
                    return Result<ProductDetailDto>.Failure("Ürün bulunamadı", 404);
                }

                var productDto = mapper.Map<ProductDetailDto>(result.Data);
                return Result<ProductDetailDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDetailDto>.Failure($"Hata: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Ürünü günceller (Edit işlemi)
        /// Admin tarafından kullanılır
        /// </summary>
        public async Task<IResult> UpdateProductAsync(int id, ProductDetailDto dto)
        {
            try
            {
                // Ürünü bul
                var result = await unitOfWork.ProductRepository.FindFirstAsync(
                    x => x.Id == id,
                    "SubCategory", "Brand", "Images", "ProductReviews"
                );

                if (!result.IsSuccess)
                {
                    return Result.Failure("Ürün bulunamadı", 404);
                }

                var product = result.Data;

                // Güncellenecek alanları ayarla
                product.Name = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.StockQuantity = dto.Stock;
                product.Active = dto.Active;
                product.UpdatedAt = DateTime.UtcNow;

                // Güncelle
                var updateResult = await unitOfWork.ProductRepository.UpdateAsync(product);

                if (!updateResult.IsSuccess)
                {
                    return Result.Failure("Ürün güncellenemedi", 500);
                }

                // Veritabanına kaydet
                await unitOfWork.SaveChangesAsync();

                return Result.Success(200, "Ürün başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Hata: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Ürünü siler (soft delete - Deleted flag'ı true yapılır)
        /// Admin tarafından kullanılır
        /// </summary>
        public async Task<IResult> DeleteProductAsync(int id)
        {
            try
            {
                // Ürünü bul
                var result = await unitOfWork.ProductRepository.FindFirstAsync(
                    x => x.Id == id
                );

                if (!result.IsSuccess)
                {
                    return Result.Failure("Ürün bulunamadı", 404);
                }

                var product = result.Data;

                // Soft Delete - Deleted flag'ını true yap
                product.Deleted = true;
                product.UpdatedAt = DateTime.UtcNow;

                // Güncelle
                var updateResult = await unitOfWork.ProductRepository.UpdateAsync(product);

                if (!updateResult.IsSuccess)
                {
                    return Result.Failure("Ürün silinemedi", 500);
                }

                // Veritabanına kaydet
                await unitOfWork.SaveChangesAsync();

                return Result.Success(200, "Ürün başarıyla silindi");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Hata: {ex.Message}", 500);
            }
        }
    }
}