using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.data;
using NotificationAPI.DTOs;
using NotificationAPI.Interfaces;
using NotificationAPI.Models;
using NotificationAPI.SignalRHub;

namespace NotificationAPI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHubContext<ChatHub> _chatHubContext;


        public NotificationService(AppDbContext appDbContext, IMapper mapper,
        IHubContext<NotificationHub> hubContext, IUserService userService, IHubContext<ChatHub> chatHubContext)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _hubContext = hubContext;
            _chatHubContext = chatHubContext;
            _userService = userService;
        }

        // public async Task<PaginatedResult<CategoryReadDto>> GetAllCategories(int pageNumber, int pageSize, string? search = null, string? sortOrder = null) {
        //     // return _categories.Select(c => new CategoryReadDto {
        //     //     CategoryId = c.CategoryId,
        //     //     Name = c.Name,
        //     //     Description = c.Description
        //     //     // CreatedAt = c.CreatedAt
        //     // }).ToList();

        //     IQueryable<Category> query = _appDbContext.Categories;

        //     if(!string.IsNullOrWhiteSpace(search)) {
        //         var formattedSearch = $"%{search.Trim()}%";
        //         query = query.Where(c => EF.Functions.ILike(c.Name, formattedSearch) || EF.Functions.ILike(c.Description, formattedSearch));
        //     }

        //     if(string.IsNullOrWhiteSpace(sortOrder)) {
        //         query = query.OrderBy(c => c.Name);
        //     } else {
        //        var formattedSortOrder = sortOrder.Trim().ToLower();
        //        if(Enum.TryParse<SortOrder>(formattedSortOrder, true, out var parsedSortOrder)) {

        //         query = parsedSortOrder switch {
        //          SortOrder.NameAsc => query.OrderBy(c => c.Name),
        //          SortOrder.NameDesc => query.OrderByDescending(c => c.Name),
        //          SortOrder.CreatedAtAsc => query.OrderBy(c => c.CreatedAt),
        //          SortOrder.CreatedAtDesc => query.OrderByDescending(c => c.CreatedAt),
        //           _ => query.OrderBy(c => c.Name),
        //         };
        //        }
        //     }

        //     var totalCount = await query.CountAsync();
        //     var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        //     // var categories = await _appDbContext.Categories.ToListAsync();
        //     var results = _mapper.Map<List<CategoryReadDto>>(items);
        //     return new PaginatedResult<CategoryReadDto> {
        //         Items = results,
        //         TotalCount = totalCount,
        //         PageNumber = pageNumber,
        //         PageSize = pageSize
        //     };
        // }
        // public async Task<CategoryReadDto?> GetCategoryById(Guid categoryId) {

        //     var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);
        //     return foundCategory == null ? null : _mapper.Map<CategoryReadDto>(foundCategory);
        // }

        public async Task<bool> CreateNotification(EventDto notificationData)
        {
            // foreach (var userId in notificationData.UserId!)
            // {
            //     var newNotification = new Notification
            //     {
            //         NotificationId = Guid.NewGuid().ToString(),
            //         UserId = userId,
            //         Entity = notificationData.Entity!,
            //         EntityId = notificationData.EntityId!,
            //         IsRead = false,
            //         Title = notificationData.Title!,
            //         Message = notificationData.Message!,
            //         CreatedAt = DateTime.UtcNow,
            //     };
            //     await _appDbContext.Notifications.AddAsync(newNotification);
            //     await _appDbContext.SaveChangesAsync();
            // }

            if (notificationData.Whom == "Admin")
            {
                IEnumerable<AdminDto> admins = await _userService.GetAdmins();
                foreach (var admin in admins)
                {
                    var newNotification = new Notification
                    {
                        NotificationId = Guid.NewGuid().ToString(),
                        UserId = admin.UserId,
                        Entity = notificationData.Entity!,
                        EntityId = notificationData.EntityId!,
                        IsRead = false,
                        Title = notificationData.Title!,
                        Message = notificationData.Message!,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _appDbContext.Notifications.AddAsync(newNotification);
                    await _appDbContext.SaveChangesAsync();
                }
                await _hubContext.Clients.Group("admin").SendAsync("ReceiveMessage", "DemoMessage");
            }
            else if (notificationData.Whom == "User")
            {
                foreach (var userId in notificationData.UserId!)
                {
                    var newNotification = new Notification
                    {
                        NotificationId = Guid.NewGuid().ToString(),
                        UserId = userId,
                        Entity = notificationData.Entity!,
                        EntityId = notificationData.EntityId!,
                        IsRead = false,
                        Title = notificationData.Title!,
                        Message = notificationData.Message!,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _appDbContext.Notifications.AddAsync(newNotification);
                    await _appDbContext.SaveChangesAsync();
                    await _hubContext.Clients.Group($"user:{userId}").SendAsync("ReceiveMessage", "DemoMessage");
                }
            }
            return true;
        }

    }


}