using CartAPI.DTOs;
using CartAPI.Extensions;
using CartAPI.Services;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Controllers;

namespace CartAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = User.GetUserId();
            var result = await _cartService.AddToCart(dto, userId);
            return ApiResponse.Success(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = User.GetUserId();
            var result = await _cartService.GetUserCart(userId);
            return ApiResponse.Success(result);
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCartItem(string cartId, [FromBody] UpdateCartItemDto dto)
        {
            var result = await _cartService.UpdateCartItem(cartId, dto);
            return result != null ? ApiResponse.Success(result) : NotFound();
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveCartItem(string cartId)
        {
            var result = await _cartService.RemoveCartItem(cartId);
            return result ? ApiResponse.Success("Removed from cart") : NotFound();
        }
    }
}
