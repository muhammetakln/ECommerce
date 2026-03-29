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

        public async Task<IResult<IEnumerable<ProductListItemDto>>> GetProductAsync()
        {
            var result = await unitOfWork.ProductRepository.FindManyAsync(
                x => x.Active && !x.Deleted,
                "SubCategory", "Brand", "Images", "ProductReviews"
            );

            if (!result.IsSuccess)
            {
                // ✅ Artık doğrudan çalışır
                return Result<IEnumerable<ProductListItemDto>>.Failure(
                    result.Message ?? "Ürünler yüklenemedi",
                    result.StatusCode
                );
            }

            var products = mapper.Map<IEnumerable<ProductListItemDto>>(result.Data);
            return Result<IEnumerable<ProductListItemDto>>.Success(products, 200, "Ürünler yüklendi");
        }

        public async Task<IResult<ProductDetailDto>> GetProductByIdAsync(int id)
        {
            var result = await unitOfWork.ProductRepository.FindManyAsync(
                x => x.Id == id && x.Active && !x.Deleted,
                "SubCategory", "Brand", "Images", "ProductReviews"
            );
            if (!result.IsSuccess)
            {
                return Result<ProductDetailDto>.Failure(
                    result.Message ?? "Ürün bulunamadı",
                    result.StatusCode
                );
            }
            var product = result.Data.FirstOrDefault();
            return Result<ProductDetailDto>.Success(
                mapper.Map<ProductDetailDto>(product),
                200,
                "Ürün detayları yüklendi"
            );


        }



      
           public async Task<IResult<ProductDetailDto>> GetProductDetailAsync(int id)
        {
            try
            {
                // ✅ FindFirstAsync kullan (tek ürün getir)
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
                    return Result<ProductDetailDto>.Failure(
                        "Ürün bulunamadı",
                        404
                    );
                }

                var productDto = mapper.Map<ProductDetailDto>(result.Data);
                return Result<ProductDetailDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDetailDto>.Failure(
                    $"Hata: {ex.Message}",
                    500
                );
            }
        }
    }
}



