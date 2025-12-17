using System;
using System.Windows;
using BadBoys.BLL.Services;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;
        
        public LoginWindow(UserService userService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userService = userService;
            _serviceProvider = serviceProvider;
            
            // For testing, you can pre-fill credentials
            txtUsername.Text = "test";
            txtPassword.Password = "test123";
        }
        
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || 
                    string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Please enter username and password", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                btnLogin.IsEnabled = false;
                
                var user = await _userService.LoginAsync(txtUsername.Text, txtPassword.Password);
                
                if (user != null)
                {
                    // Store current user globally
                    App.CurrentUser = user;
                    
                    // Create main window
                    var mainWindow = _serviceProvider.GetService(typeof(MainWindow)) as MainWindow;
                    mainWindow?.Show();
                    
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password", 
                        "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }
        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var registerWindow = new RegisterWindow(_userService);
                registerWindow.Owner = this;
                registerWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening registration: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
