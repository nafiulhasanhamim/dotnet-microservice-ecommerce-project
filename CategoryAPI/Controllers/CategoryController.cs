using CategoryAPI.DTOs;
using CategoryAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CategoryAPI.Controllers
{
    [ApiController]
    [Route("api/categories/")]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 5, [FromQuery] string? search = null, [FromQuery] string? sortOrder = null)
        {
            //  if(!string.IsNullOrEmpty(searchValue)) { 
            //     var searchCategories = categories.Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
            //     return Ok(searchCategories);
            //    }
            var categoryList = await _categoryService.GetAllCategories(PageNumber, PageSize, search, sortOrder);
            // return Ok(categoryList);
            return ApiResponse.Success(categoryList, "Categories are returned succesfully");

        }

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(string categoryId)
        {
            var category = await _categoryService.GetCategoryById(categoryId);
            if (category == null)
            {
                return ApiResponse.NotFound("Category with this id is not found");
            }
            return ApiResponse.Success(category, "Categories with this id returned succesfully");
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromQuery] CategoryCreateDto categoryData)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse.BadRequest("Invalid category Data");
            }

            var categoryReadDto = await _categoryService.CreateCategory(categoryData);
            //  return Created($"/api/categories/{newCategory.CategoryId}", categoryReadDto);
            return ApiResponse.Created(categoryReadDto, "Category is created");
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategoryById(string categoryId, [FromBody] CategoryUpdateDto categoryData)
        {
            var foundCategory = await _categoryService.UpdateCategoryById(categoryId, categoryData);
            if (foundCategory == null)
            {
                return ApiResponse.NotFound("Category with this id is not found");
            }

            if (categoryData == null)
            {
                return ApiResponse.NotFound("categoryData is missing");
            }
            return ApiResponse.Success(foundCategory, "CategoryData is updated");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategoryById(string categoryId)
        {
            var foundCategory = await _categoryService.DeleteCategoryById(categoryId);
            if (!foundCategory)
            {
                return NotFound("Category with this id is not found");
            }
            return ApiResponse.Success<object>(null, "Successfully deleted");
        }


    }
}
