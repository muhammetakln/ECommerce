using AutoMapper;
using Core.Concretes.Dtos;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;

namespace Business.Profiles
{
    public class ShopProfiles : Profile
    {
        public ShopProfiles()
        {
            CreateMap<Cart, CartDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src =>
                    src.Product != null && src.Product.Images != null
                        ? src.Product.Images.FirstOrDefault(x => x.IsCoverImage) != null
                            ? src.Product.Images.FirstOrDefault(x => x.IsCoverImage)!.ImageUrl
                            : src.Product.Images.FirstOrDefault() != null
                                ? src.Product.Images.FirstOrDefault()!.ImageUrl
                                : null
                        : null))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src =>
                    src.Product != null
                        ? src.Product.Price * (100 - src.Product.DiscountRate) / 100 : 0));
        }
    }
}