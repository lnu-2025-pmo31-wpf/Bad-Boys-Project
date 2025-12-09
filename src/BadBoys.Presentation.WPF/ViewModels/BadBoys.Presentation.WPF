using System.Windows;
using BadBoys.Presentation.WPF.ViewModels;

namespace BadBoys.Presentation.WPF
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainViewModel)DataContext;
            _vm.OnShowMedia = () => MainFrame.Navigate(new Views.MediaListPage());
            _vm.OnShowTags = () => MessageBox.Show("Tags page not implemented yet");
            _vm.OnShowLogs = () => MessageBox.Show("Logs page not implemented yet");
        }

        private void Exit_Click(object sender, RoutedEventArgs e) => Close();
        private void OpenProfile_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Profile - not implemented");
        private void Logout_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Logout - not implemented");
    }
}
