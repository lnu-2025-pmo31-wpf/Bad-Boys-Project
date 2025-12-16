using System.Windows;
using BadBoys.Presentation.WPF.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BadBoys.Presentation.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            var page = App.Services.GetRequiredService<MediaListPage>();
            MainFrame.Navigate(page);
        }
    }
}
