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
        public static string DatabasePath { get; private set; } = "";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Console.WriteLine("=== STARTING ===");
                
                // Set database path - USE CURRENT DIRECTORY
                DatabasePath = "media.db";  // SIMPLE - in current directory
                Console.WriteLine($"Database: {DatabasePath}");
                Console.WriteLine($"Full path: {Path.GetFullPath(DatabasePath)}");
                
                // Setup services
                var services = new ServiceCollection();
                
                // SIMPLE connection string
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite($"Data Source={DatabasePath}"));
                    
                services.AddScoped<UserService>();
                services.AddScoped<MediaService>();
                
                services.AddSingleton<MainWindow>();
                services.AddSingleton<LoginWindow>();
                services.AddTransient<RegisterWindow>();
                services.AddTransient<MediaListPage>();
                services.AddTransient<EditMediaWindow>();
                
                services.AddTransient<MainViewModel>();
                services.AddTransient<MediaListViewModel>();
                services.AddTransient<EditMediaViewModel>();
                
                Services = services.BuildServiceProvider();
                
                // MANUALLY CREATE DATABASE
                CreateDatabaseManually();
                
                // Show login
                var loginWindow = Services.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Startup Failed", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }
        
        private void CreateDatabaseManually()
        {
            Console.WriteLine("=== CREATING DATABASE ===");
            
            // Delete if exists
            if (File.Exists(DatabasePath))
            {
                Console.WriteLine("Deleting old database...");
                File.Delete(DatabasePath);
            }
            
            // Create database file
            Console.WriteLine($"Creating database at: {Path.GetFullPath(DatabasePath)}");
            
            // Use DbContext to create database
            using var scope = Services!.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            // FORCE creation with raw SQL
            context.Database.EnsureDeleted(); // Make sure it's gone
            context.Database.EnsureCreated(); // Create with EF Core
            
            Console.WriteLine("Database created with EnsureCreated()");
            
            // Verify
            var tables = context.Database.SqlQueryRaw<string>(
                "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name").ToList();
            
            Console.WriteLine($"Tables created: {tables.Count}");
            foreach (var table in tables)
            {
                Console.WriteLine($"  - {table}");
            }
            
            // Add test user
            if (!context.Users.Any())
            {
                Console.WriteLine("Adding test user...");
                
                // Password hash for "test123"
                var testUser = new User
                {
                    Username = "test",
                    PasswordHash = "gRimlB3wg7/uxo9vOMyNQ5ggCHQzH0cQp4n7VFtM9jA=", // test123
                    Role = "User"
                };
                
                context.Users.Add(testUser);
                context.SaveChanges();
                
                Console.WriteLine($"User created: {testUser.Username} (ID: {testUser.Id})");
            }
            
            Console.WriteLine("=== DATABASE READY ===");
        }
    }
}
