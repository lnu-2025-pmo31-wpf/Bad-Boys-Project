using System.Linq;
using System.Threading.Tasks;
using BadBoys.DAL;
using BadBoys.DAL.Entities;
using BadBoys.BLL.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BadBoys.BLL.Tests.Services
{
    public class MediaServiceTests
    {
        private AppDbContext CreateInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMedia()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_GetAll");
            
            // Створюємо користувача для зв'язку
            var user = new User 
            { 
                Id = 1, 
                Username = "testuser", 
                PasswordHash = "hash",
                Role = "User" 
            };
            context.Users.Add(user);
            
            // Додаємо тестові дані з правильним UserId
            context.Media.Add(new Media 
            { 
                Id = 1, 
                Title = "Movie 1", 
                Type = "Movie", 
                UserId = 1  // Важливо: має відповідати Id користувача
            });
            context.Media.Add(new Media 
            { 
                Id = 2, 
                Title = "Movie 2", 
                Type = "Movie", 
                UserId = 1  // Теж саме
            });
            
            await context.SaveChangesAsync();

            var service = new MediaService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoMedia()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_GetAll_Empty");
            var service = new MediaService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddMediaToDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_Add");
            
            // Створюємо користувача
            context.Users.Add(new User 
            { 
                Id = 1, 
                Username = "testuser", 
                PasswordHash = "hash" 
            });
            await context.SaveChangesAsync();
            
            var service = new MediaService(context);
            
            var newMedia = new Media 
            { 
                Title = "New Movie", 
                Type = "Movie", 
                UserId = 1,  // Правильний UserId
                Year = 2024,
                Genre = "Action"
            };

            // Act
            await service.AddAsync(newMedia);

            // Assert
            var mediaInDb = await context.Media.FirstOrDefaultAsync(m => m.Title == "New Movie");
            Assert.NotNull(mediaInDb);
            Assert.Equal("New Movie", mediaInDb.Title);
            Assert.Equal(2024, mediaInDb.Year);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingMedia()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_Update");
            
            // Створюємо користувача
            context.Users.Add(new User { Id = 1, Username = "testuser", PasswordHash = "hash" });
            
            // Додаємо початкові дані
            var media = new Media 
            { 
                Id = 1, 
                Title = "Old Title", 
                Type = "Movie", 
                UserId = 1,
                Year = 2020
            };
            context.Media.Add(media);
            await context.SaveChangesAsync();

            var service = new MediaService(context);
            
            // Отримуємо медіа з бази та оновлюємо
            var mediaToUpdate = await context.Media.FindAsync(1);
            mediaToUpdate.Title = "Updated Title";
            mediaToUpdate.Year = 2024;

            // Act
            await service.UpdateAsync(mediaToUpdate);

            // Assert
            var updatedMedia = await context.Media.FindAsync(1);
            Assert.NotNull(updatedMedia);
            Assert.Equal("Updated Title", updatedMedia.Title);
            Assert.Equal(2024, updatedMedia.Year);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMediaFromDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_Delete");
            
            // Створюємо користувача
            context.Users.Add(new User { Id = 1, Username = "testuser", PasswordHash = "hash" });
            
            var media = new Media 
            { 
                Id = 1, 
                Title = "To Delete", 
                Type = "Movie", 
                UserId = 1 
            };
            context.Media.Add(media);
            await context.SaveChangesAsync();

            var service = new MediaService(context);
            var mediaToDelete = await context.Media.FindAsync(1);

            // Act
            await service.DeleteAsync(mediaToDelete);

            // Assert
            var deletedMedia = await context.Media.FindAsync(1);
            Assert.Null(deletedMedia);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingMedia()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_Search");
            
            // Створюємо користувача
            context.Users.Add(new User { Id = 1, Username = "testuser", PasswordHash = "hash" });
            
            context.Media.AddRange(
                new Media { Id = 1, Title = "The Matrix", Genre = "Sci-Fi", UserId = 1 },
                new Media { Id = 2, Title = "Inception", Genre = "Thriller", UserId = 1 },
                new Media { Id = 3, Title = "Interstellar", Genre = "Sci-Fi", UserId = 1 }
            );
            await context.SaveChangesAsync();

            var service = new MediaService(context);

            // Act - Пошук за назвою
            var resultByTitle = await service.SearchAsync("Matrix");
            
            // Act - Пошук за жанром
            var resultByGenre = await service.SearchAsync("Sci-Fi");

            // Assert
            Assert.Single(resultByTitle);
            Assert.Equal("The Matrix", resultByTitle[0].Title);
            
            Assert.Equal(2, resultByGenre.Count); // Matrix і Interstellar
        }

        [Fact]
        public async Task SearchAsync_WithEmptyQuery_ShouldReturnAllMedia()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_SearchEmpty");
            
            // Створюємо користувача
            context.Users.Add(new User { Id = 1, Username = "testuser", PasswordHash = "hash" });
            
            context.Media.AddRange(
                new Media { Id = 1, Title = "Movie 1", UserId = 1 },
                new Media { Id = 2, Title = "Movie 2", UserId = 1 }
            );
            await context.SaveChangesAsync();

            var service = new MediaService(context);

            // Act
            var result = await service.SearchAsync("");

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchAsync_WithNoMatches_ShouldReturnEmpty()
        {
            // Arrange
            using var context = CreateInMemoryDbContext("Test_SearchNoMatch");
            
            // Створюємо користувача
            context.Users.Add(new User { Id = 1, Username = "testuser", PasswordHash = "hash" });
            
            context.Media.Add(new Media { Id = 1, Title = "Test Movie", UserId = 1 });
            await context.SaveChangesAsync();

            var service = new MediaService(context);

            // Act
            var result = await service.SearchAsync("Nonexistent");

            // Assert
            Assert.Empty(result);
        }
    }
}




















