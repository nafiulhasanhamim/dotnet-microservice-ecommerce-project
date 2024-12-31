using CouponAPI.Controllers;
using CouponAPI.DTOs;
namespace CouponAPI.Interfaces
{
    public interface ICouponService
    {
        Task<PaginatedResult<CouponReadDto>> GetAllCoupons(int pageNumber, int pageSize, string? search = null, string? sortOrder = null);
        Task<CouponReadDto?> GetCouponById(string couponId);
        Task<CouponReadDto> CreateCoupon(CouponCreateDto couponData);
        Task<CouponReadDto?> UpdateCouponById(string couponId, CouponUpdateDto couponData);
        Task<bool> DeleteCouponById(string couponId);
    }
}