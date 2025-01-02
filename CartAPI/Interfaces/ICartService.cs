
using CartAPI.DTOs;

namespace CartAPI.Services
{
    public interface ICartService
    {
        Task<CartItemDto> AddToCart(AddToCartDto addToCartDto, string userId);
        Task<CartDto> GetUserCart(string userId);
        Task<CartItemDto> UpdateCartItem(string cartId, UpdateCartItemDto updateDto);
        Task<bool> RemoveCartItem(string cartId);
    }
}
