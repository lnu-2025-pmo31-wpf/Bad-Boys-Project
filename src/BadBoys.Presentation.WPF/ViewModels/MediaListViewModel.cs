using BadBoys.Presentation.WPF.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BadBoys.DAL.Entities;
using BadBoys.BLL.Services;
using System.Threading.Tasks;
using System.Windows;
using System;

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
            
            // Load initial data
            Task.Run(async () => await LoadAsync());
        }
        
        private async Task LoadAsync()
        {
            try
            {
                if (App.CurrentUser == null) return;
                
                Application.Current.Dispatcher.Invoke(() => Items.Clear());
                
                var list = await _mediaService.GetAllAsync();
                var userMedia = list.Where(m => m.UserId == App.CurrentUser.Id);
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in userMedia)
                        Items.Add(item);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading: {ex.Message}");
            }
        }
        
        private async Task AddAsync()
        {
            try
            {
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("Please login first!");
                    return;
                }
                
                var m = new Media
                {
                    Title = $"New Media {DateTime.Now:HHmmss}",
                    Type = "Movie",
                    Rating = 5,
                    Genre = "Unknown",
                    Description = "New item",
                    Year = DateTime.Now.Year,
                    DurationMinutes = 120,
                    UserId = App.CurrentUser.Id,
                    Status = "Planned"
                };
                
                await _mediaService.AddAsync(m);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Add failed: {ex.Message}");
            }
        }
        
        private async Task EditAsync()
        {
            if (SelectedItem == null) return;
            
            try
            {
                SelectedItem.Title += " (edited)";
                await _mediaService.UpdateAsync(SelectedItem);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Edit failed: {ex.Message}");
            }
        }
        
        private async Task DeleteAsync()
        {
            if (SelectedItem == null) return;
            
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
    }
}
