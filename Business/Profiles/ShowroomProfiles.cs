using AutoMapper;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;

namespace Business.Profiles
{
    public class ShowroomProfiles : Profile
    {
        public ShowroomProfiles()
        {
            // ========== PRODUCT -> PRODUCTLISTITEMDTO (LİSTE) ==========
            CreateMap<Product, ProductListItemDto>()
                .ForMember(dest => dest.BrandName,
                    opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "Unknown"))
                .ForMember(dest => dest.SubCategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : "Unknown"))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null && src.SubCategory.Category != null
                        ? src.SubCategory.Category.Name
                        : "Unknown"))
                .ForMember(dest => dest.CoverImageUrl,
                    opt => opt.MapFrom(src => src.Images != null && src.Images.Any(x => x.IsCoverImage)
                        ? src.Images.FirstOrDefault(x => x.IsCoverImage)!.ImageUrl
                        : null))
                .ForMember(dest => dest.ReviewCount,
                    opt => opt.MapFrom(src => src.ProductReviews != null ? src.ProductReviews.Count : 0))
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src => src.ProductReviews != null && src.ProductReviews.Any()
                        ? src.ProductReviews.Average(x => (decimal)x.Vote)
                        : 0));

            // ========== PRODUCT -> PRODUCTDETAILDTO (DETAY) ==========
            CreateMap<Product, ProductDetailDto>()
                // Temel Bilgiler
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DiscountedPrice,
                    opt => opt.MapFrom(src => src.Price)) // Eğer özel fiyat varsa onu kullan
               .ForMember(dest => dest.Stock,
                        opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.Active,
                    opt => opt.MapFrom(src => src.Active))

                // Kategori Bilgileri
                .ForMember(dest => dest.BrandName,
                    opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : "Unknown"))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null && src.SubCategory.Category != null
                        ? src.SubCategory.Category.Name
                        : "Unknown"))
                .ForMember(dest => dest.SubCategoryName,
                    opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : "Unknown"))

                // Resim Bilgileri
                .ForMember(dest => dest.CoverImageUrl,
                    opt => opt.MapFrom(src => src.Images != null && src.Images.Any(x => x.IsCoverImage)
                        ? src.Images.FirstOrDefault(x => x.IsCoverImage)!.ImageUrl
                        : null))
                .ForMember(dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.Images != null && src.Images.Any()
                        ? src.Images.Select(x => x.ImageUrl).ToList()
                        : new List<string>()))

                // Rating Bilgileri
                .ForMember(dest => dest.ReviewCount,
                    opt => opt.MapFrom(src => src.ProductReviews != null ? src.ProductReviews.Count : 0))
                .ForMember(dest => dest.Rating,
                    opt => opt.MapFrom(src => src.ProductReviews != null && src.ProductReviews.Any()
                        ? src.ProductReviews.Average(x => (decimal)x.Vote)
                        : 0))

                // Yorum Bilgileri
                .ForMember(dest => dest.Reviews,
                    opt => opt.MapFrom(src => src.ProductReviews != null
                        ? src.ProductReviews.ToList()
                        : new List<ProductReview>()));

            // ========== PRODUCTREVIEW -> PRODUCTREVIEWDTO ==========
            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Comment,
                    opt => opt.MapFrom(src => src.Review))
                .ForMember(dest => dest.Vote,
                    opt => opt.MapFrom(src => src.Vote))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer != null ? src.Customer.UserName : "Anonim"));
        }
    }
}