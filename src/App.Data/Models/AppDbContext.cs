using Microsoft.EntityFrameworkCore;
using App.Data.Models;

namespace App.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Log> Logs { get; set; }

        private readonly string _connectionString;

        public AppDbContext()
        {
            // default path - файл media.db у корені проекту Console.EFTest при запуску
            _connectionString = "Data Source=media.db";
        }

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString);

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
