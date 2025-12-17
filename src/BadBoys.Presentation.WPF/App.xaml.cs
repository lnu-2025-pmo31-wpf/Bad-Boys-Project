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

            // Global exception handling
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show($"Unhandled error: {ex?.Message}",
                    "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show($"UI Error: {args.Exception.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            try
            {
                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                Services = serviceCollection.BuildServiceProvider();

                // Ensure database exists
                EnsureDatabaseExists();

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

        private void EnsureDatabaseExists()
        {
            try
            {
                // First, copy database from project root to output directory if needed
                var projectRootDb = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "media.db");
                var outputDb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
                
                if (File.Exists(projectRootDb) && (!File.Exists(outputDb) || new FileInfo(projectRootDb).Length > new FileInfo(outputDb).Length))
                {
                    File.Copy(projectRootDb, outputDb, true);
                    Console.WriteLine($"Copied database from project root to output directory");
                }
                
                var dbPath = outputDb;
                
                if (!File.Exists(dbPath))
                {
                    MessageBox.Show($"Database not found at: {dbPath}\nCreating new database...");
                }

                using var scope = Services!.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Ensure database is created
                dbContext.Database.EnsureCreated();

                // Seed the database
                SeedDatabase(dbContext);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}",
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SeedDatabase(AppDbContext context)
        {
            // Check if users already exist
            if (!context.Users.Any())
            {
                // Create test user (password: test123)
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var bytes = System.Text.Encoding.UTF8.GetBytes("test123");
                var hash = sha256.ComputeHash(bytes);
                var hashString = Convert.ToBase64String(hash);

                var testUser = new User
                {
                    Username = "test",
                    PasswordHash = hashString,
                    Role = "User"
                };

                context.Users.Add(testUser);
                context.SaveChanges();

                // Create test media item
                var testMedia = new Media
                {
                    Title = "The Matrix",
                    Year = 1999,
                    Type = BadBoys.DAL.Enums.MediaType.Movie,
                    Genre = "Sci-Fi",
                    Author = "The Wachowskis",
                    Publisher = "Warner Bros.",
                    Description = "A computer hacker learns about the true nature of reality",
                    Rating = 8.7,
                    Status = BadBoys.DAL.Enums.MediaStatus.Completed,
                    PersonalNotes = "Great movie!",
                    UserId = 1,
                    DateAdded = DateTime.Now
                };

                context.Media.Add(testMedia);
                context.SaveChanges();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Use database in output directory (where the app runs)
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
            
            // Debug output
            Console.WriteLine($"Database path: {dbPath}");
            Console.WriteLine($"Database exists: {File.Exists(dbPath)}");
            
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
