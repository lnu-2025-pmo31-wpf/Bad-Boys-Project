using System.Windows.Input;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MainViewModel
    {
        public ICommand ShowMediaCommand { get; }
        public ICommand ShowTagsCommand { get; }
        public ICommand ShowLogsCommand { get; }

        public MainViewModel()
        {
            ShowMediaCommand = new RelayCommand(_ => OnShowMedia?.Invoke());
            ShowTagsCommand = new RelayCommand(_ => OnShowTags?.Invoke());
            ShowLogsCommand = new RelayCommand(_ => OnShowLogs?.Invoke());
        }

        // прості callback-и, які MainWindow підключає до навігації
        public Action? OnShowMedia;
        public Action? OnShowTags;
        public Action? OnShowLogs;
    }
}
