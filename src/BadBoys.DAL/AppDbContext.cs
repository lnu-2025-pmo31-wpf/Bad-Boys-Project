using Microsoft.EntityFrameworkCore;
using BadBoys.DAL.Entities;

namespace BadBoys.DAL;

public class AppDbContext : DbContext
{
    public DbSet<Media> Media { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=media.db");
    }
}
