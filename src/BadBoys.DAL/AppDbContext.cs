using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Media> Media { get; set; }
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
