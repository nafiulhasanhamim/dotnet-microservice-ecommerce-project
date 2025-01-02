using AutoMapper;
using CartAPI.DTOs;
using CartAPI.Interfaces;
using CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;


        public CartService(AppDbContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<CartItemDto> AddToCart(AddToCartDto addToCartDto, string userId)
        {
            var product = await _productService.GetProduct(addToCartDto.ProductId);
            var existingItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == addToCartDto.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
                existingItem.Price = product.Data.Price - product.Data.Discount;
                existingItem.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return _mapper.Map<CartItemDto>(existingItem);
            }

            var cartItem = _mapper.Map<CartItem>(addToCartDto);
            cartItem.AddedAt = DateTime.UtcNow;
            cartItem.UserId = userId;
            cartItem.Price = product.Data.Price - product.Data.Discount;
            cartItem.UpdatedAt = DateTime.UtcNow;
            cartItem.CartId = Guid.NewGuid().ToString();

            await _context.Carts.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return _mapper.Map<CartItemDto>(cartItem);
        }

        public async Task<CartDto> GetUserCart(string userId)
        {
            var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            return _mapper.Map<CartDto>(cartItems);
        }

        public async Task<CartItemDto> UpdateCartItem(string cartId, UpdateCartItemDto updateDto)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null || cartItem.Quantity <= 0) return null;

            cartItem.Quantity = updateDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CartItemDto>(cartItem);
        }

        public async Task<bool> RemoveCartItem(string cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem == null) return false;
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
