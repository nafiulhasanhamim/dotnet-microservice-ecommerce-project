using Microsoft.EntityFrameworkCore;
using CartAPI.Models;
namespace CartAPI;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    public DbSet<CartItem> Carts { get; set; }

}
