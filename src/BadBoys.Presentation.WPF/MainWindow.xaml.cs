using System.Windows;
using BadBoys.Presentation.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BadBoys.Presentation.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Please login first");
                this.Close();
                return;
            }
            
            this.Title = $"Media Manager - {App.CurrentUser.Username}";
            LoadMediaListPage();
        }
        
        private void LoadMediaListPage()
        {
            try
            {
                if (App.Services != null)
                {
                    var page = App.Services.GetRequiredService<MediaListPage>();
                    MainFrame.Navigate(page);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading page: {ex.Message}");
            }
        }
    }
}
