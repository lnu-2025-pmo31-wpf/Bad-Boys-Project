using System;
using System.Windows;
using BadBoys.BLL.Services;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;
        
        public RegisterWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }
        
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Please enter a username", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    MessageBox.Show("Please enter a password", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    MessageBox.Show("Passwords do not match", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (txtPassword.Password.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters", 
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                btnRegister.IsEnabled = false;
                
                var success = await _userService.RegisterAsync(txtUsername.Text, txtPassword.Password);
                
                if (success)
                {
                    MessageBox.Show("Registration successful! You can now login.", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username already exists. Please choose a different username.", 
                        "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration error: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnRegister.IsEnabled = true;
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
