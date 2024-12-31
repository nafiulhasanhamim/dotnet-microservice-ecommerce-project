using Microsoft.EntityFrameworkCore;
using ProductAPI.Models;
namespace ProductAPI;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    public DbSet<Product> Products { get; set; }

}
