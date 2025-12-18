using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadBoys.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Media> Media { get; set; }
<<<<<<< HEAD
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
=======
<<<<<<< HEAD
=======
        public DbSet<Tag> Tags { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
>>>>>>> 90296bdbfb94c37c3f89782c0b3305ab2db1bf34
>>>>>>> af990ab39414d7c66ff21b591870331851dc4d49

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Explicitly set table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Media>().ToTable("Media");
            modelBuilder.Entity<Tag>().ToTable("Tags");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> af990ab39414d7c66ff21b591870331851dc4d49
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

<<<<<<< HEAD
            // Configure User->Media relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.MediaItems)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

=======
>>>>>>> 90296bdbfb94c37c3f89782c0b3305ab2db1bf34
>>>>>>> af990ab39414d7c66ff21b591870331851dc4d49
            base.OnModelCreating(modelBuilder);
        }
    }
}
