using System.Windows.Controls;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class MediaListPage : Page
    {
        public MediaListPage(MediaListViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
