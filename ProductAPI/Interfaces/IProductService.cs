
using ProductAPI.Controllers;
using ProductAPI.DTO;
using ProductAPI.DTOs;

namespace ProductAPI.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize, string? search = null, string? sortOrder = null);
        Task<ProductReadDto> GetByIdAsync(string id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto productDto);
        Task<ProductReadDto> UpdateAsync(string id, ProductUpdateDto productDto);
        Task<bool> DeleteAsync(string id);
        // Task<bool> UpdateStockAsync(OrderDTO eventMessage);
    }
}