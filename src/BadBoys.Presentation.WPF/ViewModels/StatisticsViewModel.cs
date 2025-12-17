using System.Collections.ObjectModel;
using System.Windows.Input;
using BadBoys.BLL.Services;
using BadBoys.Presentation.WPF.Commands;
using System.Windows;
using BadBoys.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly MediaService _mediaService;
        private Dictionary<string, int> _stats = new();
        private Dictionary<string, int> _genreStats = new();

        public ObservableCollection<StatItem> StatItems { get; } = new();
        public ObservableCollection<StatItem> GenreStatItems { get; } = new();
        public ObservableCollection<Media> TopRatedItems { get; } = new();
        
        public ICommand RefreshCommand { get; }

        // Parameterless constructor for XAML
        public StatisticsViewModel() : this(null!)
        {
        }

        public StatisticsViewModel(MediaService mediaService)
        {
            // Get service from DI if not provided
            if (mediaService == null && App.Services != null)
            {
                mediaService = App.Services.GetRequiredService<MediaService>();
            }
            
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            
            RefreshCommand = new RelayCommand(async _ => await LoadStatsAsync());
            
            Task.Run(async () => await LoadStatsAsync());
        }

        private async Task LoadStatsAsync()
        {
            if (App.CurrentUser == null) return;
            
            try
            {
                _stats = await _mediaService.GetStatisticsAsync(App.CurrentUser.Id);
                _genreStats = await _mediaService.GetGenreStatisticsAsync(App.CurrentUser.Id);
                var topRated = await _mediaService.GetTopRatedAsync(App.CurrentUser.Id, 5);
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Load general statistics
                    StatItems.Clear();
                    foreach (var stat in _stats)
                    {
                        StatItems.Add(new StatItem { Name = stat.Key, Value = stat.Value });
                    }

                    // Load genre statistics
                    GenreStatItems.Clear();
                    foreach (var stat in _genreStats.OrderByDescending(x => x.Value))
                    {
                        GenreStatItems.Add(new StatItem { Name = stat.Key, Value = stat.Value });
                    }

                    // Load top rated
                    TopRatedItems.Clear();
                    foreach (var media in topRated)
                    {
                        TopRatedItems.Add(media);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}");
            }
        }
    }

    public class StatItem
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
