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
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product.Images.FirstOrDefault(x => x.IsCoverImage)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.Product.Price * (100 - src.Product.DiscountRate) / 100));
        }
    }
}