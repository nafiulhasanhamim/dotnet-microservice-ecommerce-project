
using CartAPI.DTO;

namespace CartAPI.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> GetProduct(string id);
    }
}