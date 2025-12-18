using BadBoys.Presentation.WPF.Views;
using System.Windows;

namespace BadBoys.Presentation.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set user info
            if (App.CurrentUser != null)
            {
                UserInfoText.Text = $"Logged in as: {App.CurrentUser.Username}";
            }
            
            // Navigate to media list by default
            MediaListButton_Click(this, new RoutedEventArgs());
        }

        private void MediaListButton_Click(object sender, RoutedEventArgs e)
        {
            var page = new MediaListPage();
            MainFrame.Navigate(page);
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Statistics page will be implemented soon!", "Coming Soon", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export functionality will be implemented soon!", "Coming Soon", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Settings page will be implemented soon!", "Coming Soon", 
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            
            this.Close();
        }
    }
}
