using BadBoys.BLL.Services;
using App.Data.Models;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MediaListViewModel
    {
        private readonly MediaService _mediaService;

        public MediaListViewModel(MediaService mediaService)
        {
            _mediaService = mediaService;
        }
    }
}
