using AutoMapper;
using Core.Concretes.Dtos;
using Core.Concretes.Entities;
using System.Linq;

namespace Business.Profiles
{
    public class OrderProfiles : Profile
    {
        public OrderProfiles()
        {
            // ============================================
            // ORDER MAPPING
            // ============================================
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // ============================================
            // ORDERITEM MAPPING
            // ============================================
            CreateMap<OrderITem, OrderItemDto>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src =>
                    src.Product != null ? src.Product.Name : ""))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src =>
                    src.Product != null && src.Product.Images != null && src.Product.Images.Any(x => x.IsCoverImage)
                        ? src.Product.Images.FirstOrDefault(x => x.IsCoverImage).ImageUrl
                        : ""))
                .ForMember(dest => dest.ListPrice, opt => opt.MapFrom(src =>
                    src.ListPrice))
                .ForMember(dest => dest.DiscountValue, opt => opt.MapFrom(src =>
                    src.DiscountValue));
        }
    }
}