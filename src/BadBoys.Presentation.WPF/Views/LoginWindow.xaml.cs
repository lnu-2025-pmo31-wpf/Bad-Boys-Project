using BadBoys.BLL.Services;
using BadBoys.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            
            // Setup DI
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite("Data Source=media.db"));
            services.AddScoped<UserService>();
            
            var serviceProvider = services.BuildServiceProvider();
            _userService = serviceProvider.GetRequiredService<UserService>();
            
            // Set default credentials for testing
            UsernameTextBox.Text = "test";
            PasswordBox.Password = "test123";
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageText.Text = "Please enter username and password";
                return;
            }

            try
            {
                MessageText.Text = "Logging in...";
                
                // Check if database exists
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "media.db");
                if (!File.Exists(dbPath))
                {
                    MessageText.Text = "Database not found! Creating new one...";
                    // Force database creation
                    using var scope = App.Services?.CreateScope();
                    if (scope != null)
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        context.Database.EnsureCreated();
                        SeedDatabase(context);
                    }
                }

                var user = await _userService.LoginAsync(username, password);
                
                if (user != null)
                {
                    App.CurrentUser = user;
                    MessageText.Text = "Login successful!";
                    
                    // Open main window
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    
                    this.Close();
                }
                else
                {
                    MessageText.Text = "Invalid username or password";
                    
                    // Debug: Show what's in the database
                    using var scope = App.Services?.CreateScope();
                    if (scope != null)
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var users = context.Users.ToList();
                        Console.WriteLine($"Users in database: {users.Count}");
                        foreach (var u in users)
                        {
                            Console.WriteLine($"User: {u.Username}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = $"Login error: {ex.Message}";
                MessageBox.Show($"Detailed error: {ex}", "Login Error", 
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_userService);
            registerWindow.ShowDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CreateTestUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var scope = App.Services?.CreateScope();
                if (scope != null)
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    SeedDatabase(context);
                    MessageText.Text = "Test user created: test / test123";
                }
            }
            catch (Exception ex)
            {
                MessageText.Text = $"Error: {ex.Message}";
            }
        }

        private void SeedDatabase(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if users already exist
            if (!context.Users.Any())
            {
                // Create test user (password: test123)
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var bytes = System.Text.Encoding.UTF8.GetBytes("test123");
                var hash = sha256.ComputeHash(bytes);
                var hashString = Convert.ToBase64String(hash);

                var testUser = new BadBoys.DAL.Entities.User
                {
                    Username = "test",
                    PasswordHash = hashString,
                    Role = "User"
                };

                context.Users.Add(testUser);
                context.SaveChanges();

                // Create test media item
                var testMedia = new BadBoys.DAL.Entities.Media
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
            }
        }
    }
}
