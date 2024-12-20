using CategoryAPI.Controllers;
using CategoryAPI.DTOs;
namespace CategoryAPI.Interfaces
{
    public interface ICategoryService
    {
        Task<PaginatedResult<CategoryReadDto>> GetAllCategories(int pageNumber, int pageSize, string? search = null, string? sortOrder = null);
        Task<CategoryReadDto?> GetCategoryById(string categoryId);
        // CategoryReadDto GetCategoryById(string categoryId);
        Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryData);
        Task<CategoryReadDto?> UpdateCategoryById(Guid categoryId, CategoryUpdateDto categoryData);
        Task<bool> DeleteCategoryById(Guid categoryId);
    }
}