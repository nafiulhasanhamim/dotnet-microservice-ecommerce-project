using System.ComponentModel.DataAnnotations;
using CategoryAPI.Models;

namespace CategoryAPI.DTOs
{
    public class CategoryReadDto
    {
        public string? CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ParentId { get; set; } = string.Empty;
        public List<CategoryReadDto>? SubCategories { get; set; }

        public DateTime CreatedAt {get; set;}
    }

    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name {get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Category description can't exceed 500 characters")]
        public string? Description {get; set; }
        public string ParentId {get; set; } = string.Empty; 

    }
    public class CategoryUpdateDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Category description can't exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "ParentId is required")]
        public string ParentId { get; set; } = string.Empty;

    }
}