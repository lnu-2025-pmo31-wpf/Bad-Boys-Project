using BadBoys.BLL.Services;
using BadBoys.DAL.Entities;
using System.Collections.ObjectModel;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MediaListViewModel
    {
        private readonly MediaService _mediaService;

        public ObservableCollection<Media> MediaItems { get; set; }
            = new ObservableCollection<Media>();

        public MediaListViewModel(MediaService mediaService)
        {
            _mediaService = mediaService;
            LoadData();
        }

        private async void LoadData()
        {
            var items = await _mediaService.GetAllAsync();
            foreach (var m in items)
                MediaItems.Add(m);
        }
    }
}
