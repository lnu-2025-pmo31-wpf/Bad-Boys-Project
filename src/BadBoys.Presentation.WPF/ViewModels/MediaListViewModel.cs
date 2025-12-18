using Microsoft.Extensions.DependencyInjection;
using BadBoys.Presentation.WPF.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BadBoys.DAL.Entities;
using BadBoys.DAL.Enums;
using BadBoys.BLL.Services;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class MediaListViewModel : ViewModelBase
    {
        private readonly MediaService _mediaService;
        private string _searchText = "";
        private MediaType? _selectedType = null;
        private string _selectedGenre = "";
        private MediaStatus? _selectedStatus = null;
        private int? _minYear = null;
        private int? _maxYear = null;
        private double? _minRating = null;
        private double? _maxRating = 10;

        public ObservableCollection<Media> Items { get; set; } = new();
        public ObservableCollection<string> AllGenres { get; set; } = new();
        public ObservableCollection<string> AllTags { get; set; } = new();
        public Media? SelectedItem { get; set; }

        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public MediaType? SelectedType
        {
            get => _selectedType;
            set { _selectedType = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public string SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public MediaStatus? SelectedStatus
        {
            get => _selectedStatus;
            set { _selectedStatus = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public int? MinYear
        {
            get => _minYear;
            set { _minYear = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public int? MaxYear
        {
            get => _maxYear;
            set { _maxYear = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public double? MinRating
        {
            get => _minRating;
            set { _minRating = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public double? MaxRating
        {
            get => _maxRating;
            set { _maxRating = value; OnPropertyChanged(); _ = LoadAsync(); }
        }

        public Array MediaTypes { get; } = Enum.GetValues(typeof(MediaType));
        public Array StatusTypes { get; } = Enum.GetValues(typeof(MediaStatus));

        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        // Parameterless constructor for XAML
        public MediaListViewModel() : this(null!)
        {
        }

        public MediaListViewModel(MediaService mediaService)
        {
            // Get service from DI if not provided
            if (mediaService == null && App.Services != null)
            {
                mediaService = App.Services.GetRequiredService<MediaService>();
            }
            
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));

            RefreshCommand = new RelayCommand(async _ => await LoadAsync());
            AddCommand = new RelayCommand(_ => AddNew());
            EditCommand = new RelayCommand(_ => EditSelected(), _ => SelectedItem != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedItem != null);
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());

            Task.Run(async () =>
            {
                await LoadAsync();
                await LoadGenresAsync();
                await LoadTagsAsync();
            });
        }

        private async Task LoadAsync()
        {
            try
            {
                if (App.CurrentUser == null) return;

                Application.Current.Dispatcher.Invoke(() => Items.Clear());

                var list = await _mediaService.GetFilteredAsync(
                    App.CurrentUser.Id,
                    SearchText,
                    SelectedType,
                    string.IsNullOrWhiteSpace(SelectedGenre) ? null : SelectedGenre,
                    MinYear,
                    MaxYear,
                    SelectedStatus,
                    MinRating,
                    MaxRating);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in list)
                        Items.Add(item);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading: {ex.Message}");
            }
        }

        private async Task LoadGenresAsync()
        {
            if (App.CurrentUser == null) return;

            var genres = await _mediaService.GetAllGenresAsync(App.CurrentUser.Id);
            Application.Current.Dispatcher.Invoke(() =>
            {
                AllGenres.Clear();
                AllGenres.Add("All Genres");
                foreach (var genre in genres)
                    AllGenres.Add(genre);
            });
        }

        private async Task LoadTagsAsync()
        {
            if (App.CurrentUser == null) return;

            var tags = await _mediaService.GetAllTagsAsync(App.CurrentUser.Id);
            Application.Current.Dispatcher.Invoke(() =>
            {
                AllTags.Clear();
                foreach (var tag in tags)
                    AllTags.Add(tag);
            });
        }



private void AddNew()
{
    try
    {
        if (App.CurrentUser == null)
        {
            MessageBox.Show("Please login first!");
            return;
        }

        var editWindow = new Views.EditMediaWindow();
        var vm = new EditMediaViewModel(_mediaService)
        {
            // Set default values
            Title = "New Media Item",
            Year = DateTime.Now.Year,
            Type = BadBoys.DAL.Enums.MediaType.Movie,
            Genre = "Unknown",
            Author = "Unknown",
            Publisher = "Unknown",
            Status = BadBoys.DAL.Enums.MediaStatus.Planned,
            Rating = 5.0
        };
        
        editWindow.DataContext = vm;

        if (editWindow.ShowDialog() == true)
        {
            _ = LoadAsync();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Add failed: {ex.Message}", "Error", 
                       MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

        private void EditSelected()
        {
            if (SelectedItem == null) return;

            try
            {
                var editWindow = new Views.EditMediaWindow();
                var vm = new EditMediaViewModel(_mediaService, SelectedItem);
                editWindow.DataContext = vm;

                if (editWindow.ShowDialog() == true)
                {
                    _ = LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Edit failed: {ex.Message}");
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedItem == null) return;

            var result = MessageBox.Show(
                $"Delete '{SelectedItem.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _mediaService.DeleteAsync(SelectedItem);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Delete failed: {ex.Message}");
            }
        }

        private void ClearFilters()
        {
            SearchText = "";
            SelectedType = null;
            SelectedGenre = "";
            SelectedStatus = null;
            MinYear = null;
            MaxYear = null;
            MinRating = null;
            MaxRating = 10;
        }
    }
}
