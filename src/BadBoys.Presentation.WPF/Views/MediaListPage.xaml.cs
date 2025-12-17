using BadBoys.Presentation.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class MediaListPage : Page
    {
        public MediaListPage()
        {
            InitializeComponent();
            
            // Set DataContext from DI
            if (App.Services != null)
            {
                DataContext = App.Services.GetRequiredService<MediaListViewModel>();
            }
        }
    }
}
