using AutoMapper;
using CouponAPI.DTOs;
using CouponAPI.Models;

namespace CouponAPI.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponReadDto>();
            CreateMap<CouponCreateDto, Coupon>();
            CreateMap<CouponUpdateDto, Coupon>();
        }
    }
}