using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Media> Media { get; set; }
<<<<<<< HEAD
=======
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
>>>>>>> 90296bdbfb94c37c3f89782c0b3305ab2db1bf34

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

<<<<<<< HEAD
=======
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

>>>>>>> 90296bdbfb94c37c3f89782c0b3305ab2db1bf34
            base.OnModelCreating(modelBuilder);
        }
    }
}
