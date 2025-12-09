using Microsoft.EntityFrameworkCore;
using App.Data.Models;

namespace App.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<MediaItem> MediaItems => Set<MediaItem>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Log> Logs => Set<Log>();

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlite("Data Source=media.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.MediaItems)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MediaItem>()
                .HasMany(m => m.Tags)
                .WithOne(t => t.MediaItem)
                .HasForeignKey(t => t.MediaItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Logs)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<MediaItem>()
                .HasMany(m => m.Logs)
                .WithOne(l => l.MediaItem)
                .HasForeignKey(l => l.MediaItemId);
        }
    }
}
