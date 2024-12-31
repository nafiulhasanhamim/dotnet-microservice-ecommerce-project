using System.ComponentModel.DataAnnotations;
namespace CouponAPI.DTOs
{
    public class CouponReadDto
    {
        public string? CouponId { get; set; }
        public string? CouponName { get; set; }
        public bool Status { get; set; }
        public decimal MinimumShoppingAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CouponCreateDto
    {
        [Required(ErrorMessage = "Coupon name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Coupon name must be between 2 and 100 characters")]
        public string CouponName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public decimal MinimumShoppingAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
    public class CouponUpdateDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string CouponName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public decimal MinimumShoppingAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}