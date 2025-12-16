using BadBoys.Presentation.WPF.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BadBoys.DAL.Entities;
using BadBoys.BLL.Services;
using System.Threading.Tasks;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MediaListViewModel : ViewModelBase
    {
        private readonly MediaService _mediaService;

        public ObservableCollection<Media> Items { get; set; } = new();
        public Media? SelectedItem { get; set; }

        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public MediaListViewModel(MediaService mediaService)
        {
            _mediaService = mediaService;

            RefreshCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(async _ => await AddAsync());
            EditCommand = new RelayCommand(async _ => await EditAsync(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedItem != null);

            Task.Run(LoadAsync);
        }

        private async Task LoadAsync()
        {
            Items.Clear();
            var list = await _mediaService.GetAllAsync();
            foreach (var item in list)
                Items.Add(item);
        }

        private async Task AddAsync()
        {
            var m = new Media
            {
                Title = "New Media",
                Type = "Unknown",
                Rating = 0,
                Genre = "",
                Description = "",
                Year = 0,
                DurationMinutes = 0
            };

            await _mediaService.AddAsync(m);
            await LoadAsync();
        }

        private async Task EditAsync()
        {
            if (SelectedItem == null) return;

            SelectedItem.Title += " (edited)";
            await _mediaService.UpdateAsync(SelectedItem);
            await LoadAsync();
        }

        private async Task DeleteAsync()
        {
            if (SelectedItem == null) return;

            await _mediaService.DeleteAsync(SelectedItem);
            await LoadAsync();
        }
    }
}
