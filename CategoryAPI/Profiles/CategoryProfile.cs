using AutoMapper;
using CategoryAPI.DTOs;
using CategoryAPI.Models;

namespace CategoryAPI.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryReadDto>()
                       .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories));
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}