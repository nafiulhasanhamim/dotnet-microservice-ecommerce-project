using AutoMapper;
using CategoryAPI.Controllers;
using CategoryAPI.data;
using CategoryAPI.DTOs;
using CategoryAPI.Enums;
using CategoryAPI.Interfaces;
using CategoryAPI.Models;
using CategoryAPI.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace CategoryAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IRabbmitMQCartMessageSender _messagebus;

        public CategoryService(AppDbContext appDbContext, IMapper mapper,
        IRabbmitMQCartMessageSender messageBus
        )
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _messagebus = messageBus;
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
            // _messagebus.SendMessage(new { UserId = new List<string> { "fjfjfjfjf", "fhfhfh" }, Entity = "category", EntityId = "12234ghjhgjh4", Title = "Categories", Message = "Category Fetched", Whom = "User"}, "sentNotification", "queue");
            // _messagebus.SendMessage(new { UserId = new List<string> {}, Entity = "category", EntityId = "12234ghjhgjh4", Title = "Categories", Message = "Category Fetched", Whom = "Admin"}, "sentNotification", "queue");
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
            return foundCategory == null ? null : _mapper.Map<CategoryReadDto>(foundCategory);
        }

        public async Task<CategoryReadDto> CreateCategory(CategoryCreateDto categoryData)
        {
            var newCategory = _mapper.Map<Category>(categoryData);
            newCategory.CategoryId = Guid.NewGuid().ToString();
            newCategory.CreatedAt = DateTime.UtcNow;

            await _appDbContext.Categories.AddAsync(newCategory);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<CategoryReadDto>(newCategory);

        }
        public async Task<CategoryReadDto?> UpdateCategoryById(string categoryId, CategoryUpdateDto categoryData)
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

        public async Task<bool> DeleteCategoryById(string categoryId)
        {
            var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
            if (foundCategory == null)
            {
                return false;
            }
            _appDbContext.Categories.Remove(foundCategory);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> IsCategoryValid(EventDto eventMessage)
        {
            var foundCategory = await _appDbContext.Categories.FindAsync(eventMessage.CId);
            if (foundCategory == null)
            {
                _messagebus.SendMessage(eventMessage, "categoryCheckFailed", "queue");
                return false;
            }
            _messagebus.SendMessage(eventMessage, "categoryCheckSuccess", "queue");
            return true;
        }

    }

}

