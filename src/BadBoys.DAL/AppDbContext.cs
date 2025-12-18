using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicitly set table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Media>().ToTable("Media");
            modelBuilder.Entity<Tag>().ToTable("Tags");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Media>()
                .Property(m => m.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Media>()
                .Property(m => m.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Name);

            modelBuilder.Entity<Media>()
                .HasMany(m => m.Tags)
                .WithOne(t => t.Media)
                .HasForeignKey(t => t.MediaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User->Media relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.MediaItems)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
