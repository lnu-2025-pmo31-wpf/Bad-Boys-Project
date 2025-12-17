using BadBoys.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BadBoys.DAL
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if users already exist
            if (!context.Users.Any())
            {
                // Create test user (password: test123)
                var testUser = new User
                {
                    Username = "test",
                    PasswordHash = HashPassword("test123"),
                    Role = "User"
                };

                context.Users.Add(testUser);
                context.SaveChanges();

                Console.WriteLine("Test user created: test / test123");
            }

            // Check if media items exist
            if (!context.Media.Any())
            {
                var testMedia = new Media
                {
                    Title = "The Matrix",
                    Year = 1999,
                    Type = BadBoys.DAL.Enums.MediaType.Movie,
                    Genre = "Sci-Fi",
                    Author = "The Wachowskis",
                    Description = "A computer hacker learns about the true nature of reality",
                    Rating = 8.7,
                    Status = BadBoys.DAL.Enums.MediaStatus.Completed,
                    PersonalNotes = "Great movie!",
                    UserId = 1
                };

                context.Media.Add(testMedia);
                context.SaveChanges();

                Console.WriteLine("Test media item created");
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
