using BadBoys.Presentation.WPF.Commands;
using System.Windows.Input;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MainViewModel
    {
        public ICommand ShowMediaCommand { get; }
        public ICommand ShowTagsCommand { get; }
        public ICommand ShowLogsCommand { get; }

        public Action? OnShowMedia { get; set; }
        public Action? OnShowTags { get; set; }
        public Action? OnShowLogs { get; set; }

        public MainViewModel()
        {
            ShowMediaCommand = new RelayCommand(_ => OnShowMedia?.Invoke());
            ShowTagsCommand = new RelayCommand(_ => OnShowTags?.Invoke());
            ShowLogsCommand = new RelayCommand(_ => OnShowLogs?.Invoke());
        }
    }
}
