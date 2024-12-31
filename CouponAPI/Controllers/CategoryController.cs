using CouponAPI.DTOs;
using CouponAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CouponAPI.Controllers
{
    [ApiController]
    [Route("api/coupons/")]
    public class CouponController : ControllerBase
    {
        private ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupons([FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5, [FromQuery] string? search = null, [FromQuery] string? sortOrder = null)
        {
            var couponList = await _couponService.GetAllCoupons(PageNumber, PageSize, search, sortOrder);
            return ApiResponse.Success(couponList, "Coupons are returned succesfully");

        }

        [HttpGet("{couponId}")]
        public async Task<IActionResult> GetCouponById(string couponId)
        {
            var coupon = await _couponService.GetCouponById(couponId);
            if (coupon == null)
            {
                return ApiResponse.NotFound("Coupon with this id is not found");
            }
            return ApiResponse.Success(coupon, "Coupon with this id returned succesfully");
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromQuery] CouponCreateDto couponData)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.BadRequest("Invalid coupon Data");
            }

            var couponReadDto = await _couponService.CreateCoupon(couponData);
            return ApiResponse.Created(couponReadDto, "Coupon is created");
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("{couponId}")]
        public async Task<IActionResult> UpdateCouponById(string couponId, [FromBody] CouponUpdateDto couponData)
        {
            var foundCoupon = await _couponService.UpdateCouponById(couponId, couponData);
            if (foundCoupon == null)
            {
                return ApiResponse.NotFound("Coupon with this id is not found");
            }

            if (couponData == null)
            {
                return ApiResponse.NotFound("couponData is missing");
            }
            return ApiResponse.Success(foundCoupon, "CouponData is updated");
        }

        // [Authorize(Roles = "Admin")]
        [HttpDelete("{couponId}")]
        public async Task<IActionResult> DeleteCouponById(string couponId)
        {
            var foundCoupon = await _couponService.DeleteCouponById(couponId);
            if (!foundCoupon)
            {
                return NotFound("Coupon with this id is not found");
            }
            return ApiResponse.Success<object>(null, "Successfully deleted");
        }


    }
}
