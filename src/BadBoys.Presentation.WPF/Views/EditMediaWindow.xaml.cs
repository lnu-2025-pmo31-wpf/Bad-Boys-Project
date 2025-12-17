using BadBoys.Presentation.WPF.ViewModels;
using System.Windows;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class EditMediaWindow : Window
    {
        public EditMediaWindow()
        {
            InitializeComponent();
        }

        public EditMediaWindow(EditMediaViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
