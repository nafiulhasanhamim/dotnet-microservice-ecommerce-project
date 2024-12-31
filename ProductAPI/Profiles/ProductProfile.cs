using AutoMapper;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace Services.ProductAPI.Profiles
{
    public class ProductProfile : Profile
    {
        
        public ProductProfile()
        {
            CreateMap<Product, ProductReadDto>();

            CreateMap<ProductCreateDto, Product>();

            CreateMap<ProductUpdateDto, Product>();

        }

    }
}
