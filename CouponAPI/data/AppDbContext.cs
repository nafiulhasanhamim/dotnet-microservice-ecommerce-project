using CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CouponAPI.data;
public class AppDbContext : DbContext

{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
    public DbSet<Coupon> Coupons { get; set; }

}
