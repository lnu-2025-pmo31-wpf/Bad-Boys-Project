using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using BadBoys.DAL;
using BadBoys.BLL.Services;
using BadBoys.DAL.Entities;
using BadBoys.Presentation.WPF.Views;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        public static User? CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Console.WriteLine("=== APPLICATION STARTING ===");
                
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                Services = serviceCollection.BuildServiceProvider();

                // Initialize database (DOES NOT DELETE EXISTING)
                InitializeDatabase();

                // Show login window
                var loginWindow = Services.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Startup error: {ex.Message}", 
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void InitializeDatabase()
        {
            try
            {
                Console.WriteLine("=== INITIALIZING DATABASE ===");
                
                using var scope = Services!.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Get database path for logging
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
                Console.WriteLine($"Database path: {dbPath}");
                Console.WriteLine($"Database exists: {File.Exists(dbPath)}");
                
                if (File.Exists(dbPath))
                {
                    Console.WriteLine("Existing database found. Ensuring it's up to date...");
                    
                    try
                    {
                        // Try to apply any pending migrations
                        context.Database.Migrate();
                        Console.WriteLine("Migrations applied (if any)");
                    }
                    catch (Exception migrateEx)
                    {
                        Console.WriteLine($"Note: Could not apply migrations: {migrateEx.Message}");
                        Console.WriteLine("Using existing schema...");
                    }
                }
                else
                {
                    Console.WriteLine("No database found. Creating new one...");
                    context.Database.EnsureCreated();
                    Console.WriteLine("Database created");
                    
                    // Add test user only for NEW database
                    AddTestUserIfNeeded(context);
                }
                
                // Log database stats
                LogDatabaseInfo(context);
                
                Console.WriteLine("=== DATABASE READY ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR initializing database: {ex.Message}");
                throw;
            }
        }

        private void AddTestUserIfNeeded(AppDbContext context)
        {
            try
            {
                if (!context.Users.Any())
                {
                    Console.WriteLine("Adding test user to new database...");
                    
                    string HashPassword(string password)
                    {
                        using var sha256 = System.Security.Cryptography.SHA256.Create();
                        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                        var hash = sha256.ComputeHash(bytes);
                        return Convert.ToBase64String(hash);
                    }
                    
                    var testUser = new User
                    {
                        Username = "test",
                        PasswordHash = HashPassword("test123"),
                        Role = "User"
                    };
                    
                    context.Users.Add(testUser);
                    context.SaveChanges();
                    
                    Console.WriteLine($"Test user created: {testUser.Username} (ID: {testUser.Id})");
                    
                    // Optional: Add a test media item
                    var testMedia = new Media
                    {
                        Title = "The Matrix",
                        Year = 1999,
                        Type = BadBoys.DAL.Enums.MediaType.Movie,
                        Genre = "Sci-Fi",
                        Author = "The Wachowskis",
                        Publisher = "Warner Bros.",
                        Description = "A computer hacker learns about reality",
                        Rating = 8.7,
                        Status = BadBoys.DAL.Enums.MediaStatus.Completed,
                        PersonalNotes = "Great movie!",
                        UserId = testUser.Id,
                        DateAdded = DateTime.Now
                    };
                    
                    context.Media.Add(testMedia);
                    context.SaveChanges();
                    Console.WriteLine($"Test media created: {testMedia.Title}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Could not add test user: {ex.Message}");
            }
        }

        private void LogDatabaseInfo(AppDbContext context)
        {
            try
            {
                var userCount = context.Users.Count();
                var mediaCount = context.Media.Count();
                Console.WriteLine($"Database has {userCount} user(s) and {mediaCount} media item(s)");
                
                if (userCount > 0)
                {
                    Console.WriteLine("Users:");
                    foreach (var user in context.Users)
                    {
                        Console.WriteLine($"  {user.Id}: {user.Username}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: Could not log database info: {ex.Message}");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Use database in output directory
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
            Console.WriteLine($"Configuring database connection to: {dbPath}");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            // Register Services
            services.AddScoped<UserService>();
            services.AddScoped<MediaService>();

            // Register Windows
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<MediaListPage>();
            services.AddTransient<EditMediaWindow>();

            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<MediaListViewModel>();
            services.AddTransient<EditMediaViewModel>();
        }
    }
}
