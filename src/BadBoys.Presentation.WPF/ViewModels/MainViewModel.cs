using BadBoys.Presentation.WPF.Commands;
using System.Windows.Input;
using BadBoys.BLL.Services;

public class MainViewModel
{
    public MediaService MediaService { get; }

    public MainViewModel(MediaService mediaService)
    {
        MediaService = mediaService;
    }
}
