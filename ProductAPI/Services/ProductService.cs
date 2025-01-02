using AutoMapper;
using ProductAPI.Enums;
using Microsoft.EntityFrameworkCore;
using ProductAPI.DTO;
using ProductAPI.DTOs;
using ProductAPI.Interfaces;
using ProductAPI.Models;
using ProductAPI.RabbitMQ;
using ProductAPI.Controllers;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRabbmitMQCartMessageSender _messageBus;


        public ProductService(AppDbContext context, IMapper mapper, IRabbmitMQCartMessageSender messageBus)
        {
            _context = context;
            _mapper = mapper;
            _messageBus = messageBus;
        }

        public async Task<PaginatedResult<ProductReadDto>> GetAllAsync(int pageNumber, int pageSize, string? search = null, string? sortOrder = null)
        {
            IQueryable<Product> query = _context.Products;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var formattedSearch = $"%{search.Trim()}%";
                query = query.Where(c => EF.Functions.ILike(c.Name!, formattedSearch));
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
            var results = _mapper.Map<List<ProductReadDto>>(items);
            return new PaginatedResult<ProductReadDto>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductReadDto> GetByIdAsync(string id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null) return null;
            return _mapper.Map<ProductReadDto>(product);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.ProductId = Guid.NewGuid().ToString();
            product.CategoryVerify = true;
            product.CreatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _messageBus.SendMessage(new { PId = product.ProductId, CId = productDto.CategoryId }, "categoryCheck", "queue");
            return _mapper.Map<ProductReadDto>(product);
        }

        public async Task<ProductReadDto> UpdateAsync(string id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new KeyNotFoundException("Product not found");

            _mapper.Map(productDto, product);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductReadDto>(product);

        }

        public async Task<bool> DeleteAsync(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // public async Task<bool> UpdateStockAsync(OrderDTO eventMessage)
        // {
        //     var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == Guid.Parse(eventMessage.ProductId));
        //     if (product == null) _messageBus.SendMessage(eventMessage, "stockFailed", "exchange");
        //     else if (product.Quantity >= eventMessage.Quantity)
        //     {
        //         product.Quantity -= eventMessage.Quantity;
        //         _context.Products.Update(product);
        //         await _context.SaveChangesAsync();
        //         _messageBus.SendMessage(eventMessage, "stockUpdated", "exchange");
        //     }
        //     else if (product.Quantity < eventMessage.Quantity)
        //     {
        //         _messageBus.SendMessage(eventMessage, "stockFailed", "exchange");
        //     }
        //     return true;
        // }
    }
}
