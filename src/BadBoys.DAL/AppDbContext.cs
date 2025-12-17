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

            base.OnModelCreating(modelBuilder);
        }
    }
}
