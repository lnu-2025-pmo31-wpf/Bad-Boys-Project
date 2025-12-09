using System.ComponentModel;
using System.Windows;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF.Views
{
    public partial class EditMediaWindow : Window
    {
        public MediaItemViewModel ViewModel { get; }

        public EditMediaWindow(MediaItemViewModel? item)
        {
            InitializeComponent();
            ViewModel = item ?? new MediaItemViewModel();
            DataContext = ViewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // простий підхід: перевіряємо IDataErrorInfo
            var err = (ViewModel as IDataErrorInfo)!;
            if (!string.IsNullOrEmpty(err[nameof(ViewModel.Title)]) ||
                !string.IsNullOrEmpty(err[nameof(ViewModel.Type)]) ||
                !string.IsNullOrEmpty(err[nameof(ViewModel.Rating)]))
            {
                MessageBox.Show("Є помилки в заповненні форми.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // якщо потрібно: викликати _mediaService.Create(...) або Update(...)
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
