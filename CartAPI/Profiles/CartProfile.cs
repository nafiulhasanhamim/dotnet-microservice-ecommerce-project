using AutoMapper;
using CartAPI.DTOs;
using CartAPI.Models;

namespace ChatAPI.Profiles
{
    public class CartProfile : Profile
    {

        public CartProfile()
        {
            CreateMap<AddToCartDto, CartItem>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateCartItemDto, CartItem>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<CartItem, CartItemDto>();
            CreateMap<List<CartItem>, CartDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src));
        }
    }
}
