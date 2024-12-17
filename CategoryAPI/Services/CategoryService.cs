using AutoMapper;
using CategoryAPI.Controllers;
using CategoryAPI.data;
using CategoryAPI.DTOs;
using CategoryAPI.Enums;
using CategoryAPI.Interfaces;
using CategoryAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CategoryAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext appDbContext, IMapper mapper)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        public async Task<PaginatedResult<CategoryReadDto>> GetAllCategories(int pageNumber, int pageSize, string? search = null, string? sortOrder = null)
        {
            IQueryable<Category> query = _appDbContext.Categories.Include(c => c.SubCategories);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var formattedSearch = $"%{search.Trim()}%";
                query = query.Where(c => EF.Functions.ILike(c.Name, formattedSearch) || EF.Functions.ILike(c.Description, formattedSearch));
            }

            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                query = query.OrderBy(c => c.Name);
            }
            else
            {
                var formattedSortOrder = sortOrder.Trim().ToLower();
                if (Enum.TryParse<SortOrder>(formattedSortOrder, true, out var parsedSortOrder))
                {

                    query = parsedSortOrder switch
                    {
                        SortOrder.NameAsc => query.OrderBy(c => c.Name),
                        SortOrder.NameDesc => query.OrderByDescending(c => c.Name),
                        SortOrder.CreatedAtAsc => query.OrderBy(c => c.CreatedAt),
                        SortOrder.CreatedAtDesc => query.OrderByDescending(c => c.CreatedAt),
                        _ => query.OrderBy(c => c.Name),
                    };
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var results = _mapper.Map<List<CategoryReadDto>>(items);

            return new PaginatedResult<CategoryReadDto>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CategoryReadDto?> GetCategoryById(string categoryId)
        {
            var foundCategory = await _appDbContext.Categories
                                           .Include(c => c.SubCategories)
                                           .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            // var foundCategory = _appDbContext.Categories.Include(c => c.SubCategories);
            return foundCategory == null ? null : _mapper.Map<CategoryReadDto>(foundCategory);
        }

        public async Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryData)
        {
            var newCategory = _mapper.Map<Category>(categoryData);
            newCategory.CategoryId = Guid.NewGuid().ToString();
            newCategory.CreatedAt = DateTime.UtcNow;

            await _appDbContext.Categories.AddAsync(newCategory);
            await _appDbContext.SaveChangesAsync();

            //     return new CategoryReadDto {
            //     CategoryId = newCategory.CategoryId,
            //     Name = newCategory.Name,
            //     Description = newCategory.Description
            // };

            return _mapper.Map<CategoryReadDto>(newCategory);

        }
        public async Task<CategoryReadDto?> UpdateCategoryById(Guid categoryId, CategoryUpdateDto categoryData)
        {
            // var foundCategory = _categories.FirstOrDefault(category => category.CategoryId == categoryId);
            // var foundCategory = await _appDbContext.Categories.FirstOrDefaultAsync(category => category.CategoryId == categoryId);
            var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
            if (foundCategory == null)
            {
                return null;
            }

            //    if(!string.IsNullOrWhiteSpace(categoryData.Name)) {
            //     foundCategory.Name = categoryData.Name;
            //     }

            //    if(!string.IsNullOrWhiteSpace(categoryData.Description)) {
            //     foundCategory.Description = categoryData.Description;
            //     }

            _mapper.Map(categoryData, foundCategory);
            _appDbContext.Categories.Update(foundCategory);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<CategoryReadDto>(foundCategory);


        }

        public async Task<bool> DeleteCategoryById(Guid categoryId)
        {
            // var foundCategory = _categories.FirstOrDefault(category => category.CategoryId == categoryId);
            // var foundCategory = await _appDbContext.Categories.FirstOrDefaultAsync(category => category.CategoryId == categoryId);
            var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
            if (foundCategory == null)
            {
                return false;
            }
            _appDbContext.Categories.Remove(foundCategory);
            await _appDbContext.SaveChangesAsync();
            return true;


        }
        private List<CategoryReadDto> BuildCategoryHierarchy(List<CategoryReadDto> categories, string? parentId = null)
        {
            return categories
                .Where(c => c.ParentId == parentId)
                .Select(c => new CategoryReadDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name!,
                    Description = c.Description,
                    SubCategories = BuildCategoryHierarchy(categories, c.CategoryId)
                })
                .ToList();
        }

    }

}

