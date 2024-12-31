using AutoMapper;
using CouponAPI.Controllers;
using CouponAPI.data;
using CouponAPI.DTOs;
using CouponAPI.Enums;
using CouponAPI.Interfaces;
using CouponAPI.Models;
using CouponAPI.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace CouponAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CouponService(AppDbContext appDbContext, IMapper mapper,
        IRabbmitMQCartMessageSender messageBus
        )
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        public async Task<PaginatedResult<CouponReadDto>> GetAllCoupons(int pageNumber, int pageSize, string? search = null, string? sortOrder = null)
        {
            IQueryable<Coupon> query = _appDbContext.Coupons;

            if (!string.IsNullOrWhiteSpace(search))
            {
                var formattedSearch = $"%{search.Trim()}%";
                query = query.Where(c => EF.Functions.ILike(c.CouponName!, formattedSearch));
            }

            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                query = query.OrderBy(c => c.CouponName);
            }
            else
            {
                var formattedSortOrder = sortOrder.Trim().ToLower();
                if (Enum.TryParse<SortOrder>(formattedSortOrder, true, out var parsedSortOrder))
                {

                    query = parsedSortOrder switch
                    {
                        SortOrder.NameAsc => query.OrderBy(c => c.CouponName),
                        SortOrder.NameDesc => query.OrderByDescending(c => c.CouponName),
                        SortOrder.CreatedAtAsc => query.OrderBy(c => c.CreatedAt),
                        SortOrder.CreatedAtDesc => query.OrderByDescending(c => c.CreatedAt),
                        _ => query.OrderBy(c => c.CouponName),
                    };
                }
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var results = _mapper.Map<List<CouponReadDto>>(items);

            return new PaginatedResult<CouponReadDto>
            {
                Items = results,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<CouponReadDto?> GetCouponById(string couponId)
        {
            var foundCoupon = await _appDbContext.Coupons
                                           .FirstOrDefaultAsync(c => c.CouponId == couponId);
            return foundCoupon == null ? null : _mapper.Map<CouponReadDto>(foundCoupon);
        }

        public async Task<CouponReadDto> CreateCoupon(CouponCreateDto couponData)
        {
            var newCoupon = _mapper.Map<Coupon>(couponData);
            newCoupon.CouponId = Guid.NewGuid().ToString();
            newCoupon.CreatedAt = DateTime.UtcNow;

            await _appDbContext.Coupons.AddAsync(newCoupon);
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<CouponReadDto>(newCoupon);

        }
        public async Task<CouponReadDto?> UpdateCouponById(string couponId, CouponUpdateDto couponData)
        {
            var foundCoupon = await _appDbContext.Coupons.FindAsync(couponId);
            if (foundCoupon == null)
            {
                return null;
            }

            _mapper.Map(couponData, foundCoupon);
            _appDbContext.Coupons.Update(foundCoupon);
            await _appDbContext.SaveChangesAsync();
            return _mapper.Map<CouponReadDto>(foundCoupon);

        }

        public async Task<bool> DeleteCouponById(string couponId)
        {
            var foundCoupon = await _appDbContext.Coupons.FindAsync(couponId);
            if (foundCoupon == null)
            {
                return false;
            }
            _appDbContext.Coupons.Remove(foundCoupon);
            await _appDbContext.SaveChangesAsync();
            return true;
        }

    }

}

