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
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
                if (!File.Exists(dbPath))
                {
                    MessageBox.Show($"Database not found at: {dbPath}\nCreating new database...");
                }
                
                using var scope = Services!.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", 
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext with absolute path
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db")}"));
            
            // Register Services
            services.AddScoped<UserService>();
            services.AddScoped<MediaService>();
            
            // Register Windows
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<MediaListPage>();
            services.AddTransient<EditMediaWindow>();
            services.AddTransient<MediaListView>();
            
            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<MediaListViewModel>();
            services.AddTransient<EditMediaViewModel>();
        }
    }
}
