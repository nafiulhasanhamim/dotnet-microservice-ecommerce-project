using CategoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CategoryAPI.data;
public class AppDbContext : DbContext

{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
                .HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentId);
    }
    public DbSet<Category> Categories { get; set; }

}
