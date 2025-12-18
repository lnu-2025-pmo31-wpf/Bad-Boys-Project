using BadBoys.Presentation.WPF.Commands;
using BadBoys.DAL.Entities;
using BadBoys.DAL.Enums;
using BadBoys.BLL.Services;
using BadBoys.DAL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class EditMediaViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly MediaService _mediaService;
        private readonly Media _mediaItem;
        private bool _isNew;

        public string Title { get; set; } = "";
        public int? Year { get; set; }
        public MediaType Type { get; set; } = MediaType.Movie;
        public string Genre { get; set; } = "";
        public string Author { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string Description { get; set; } = "";
        public double? Rating { get; set; }
        public MediaStatus Status { get; set; } = MediaStatus.Planned;
        public string PersonalNotes { get; set; } = "";
        public string CoverImagePath { get; set; } = "";
        public int? DurationMinutes { get; set; }
        public int? PageCount { get; set; }
        public int? TrackCount { get; set; }
        public string NewTag { get; set; } = "";

        public ObservableCollection<string> Tags { get; set; } = new();

        public Array MediaTypes { get; } = Enum.GetValues(typeof(MediaType));
        public Array StatusTypes { get; } = Enum.GetValues(typeof(MediaStatus));

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand BrowseImageCommand { get; }

        // Parameterless constructor for XAML
        public EditMediaViewModel() : this(null!, null)
        {
        }

        public EditMediaViewModel(MediaService mediaService, Media? existingItem = null)
        {
            // Get service from DI if not provided
            if (mediaService == null && App.Services != null)
            {
                mediaService = App.Services.GetRequiredService<MediaService>();
            }

            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));

            if (existingItem != null)
            {
                _mediaItem = existingItem;
                _isNew = false;
                LoadFromEntity(existingItem);
            }
            else
            {
                _mediaItem = new Media();
                _isNew = true;
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            AddTagCommand = new RelayCommand(AddTag);
            RemoveTagCommand = new RelayCommand(RemoveTag);
            BrowseImageCommand = new RelayCommand(BrowseImage);
        }

        private void LoadFromEntity(Media media)
        {
            Title = media.Title;
            Year = media.Year;
            Type = media.Type;
            Genre = media.Genre;
            Author = media.Author;
            Publisher = media.Publisher;
            Description = media.Description;
            Rating = media.Rating;
            Status = media.Status;
            PersonalNotes = media.PersonalNotes;
            CoverImagePath = media.CoverImagePath;
            DurationMinutes = media.DurationMinutes;
            PageCount = media.PageCount;
            TrackCount = media.TrackCount;

            Tags.Clear();
            foreach (var tag in media.Tags.Select(t => t.Name))
                Tags.Add(tag);
        }

        private void SaveToEntity()
        {
            _mediaItem.Title = Title ?? "";
            _mediaItem.Year = Year;
            _mediaItem.Type = Type;
            _mediaItem.Genre = Genre ?? "";
            _mediaItem.Author = Author ?? "";
            _mediaItem.Publisher = Publisher ?? "";
            _mediaItem.Description = Description ?? "";
            _mediaItem.Rating = Rating;
            _mediaItem.Status = Status;
            _mediaItem.PersonalNotes = PersonalNotes ?? "";
            _mediaItem.CoverImagePath = CoverImagePath ?? "";
            _mediaItem.DurationMinutes = DurationMinutes;
            _mediaItem.PageCount = PageCount;
            _mediaItem.TrackCount = TrackCount;

            _mediaItem.Tags.Clear();
            foreach (var tagName in Tags)
            {
                _mediaItem.Tags.Add(new Tag { Name = tagName });
            }

            if (_isNew)
            {
                // CRITICAL FIX: Use the currently logged-in user, NOT hardcoded UserId = 1
                if (App.CurrentUser == null)
                {
                    MessageBox.Show("ERROR: No user is logged in. Cannot save media. Please login again.",
                        "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new InvalidOperationException("No user is logged in");
                }

                _mediaItem.UserId = App.CurrentUser.Id; // Use ACTUAL logged-in user ID
                _mediaItem.DateAdded = DateTime.Now;

                Console.WriteLine($"DEBUG: SaveToEntity - New media - UserId={_mediaItem.UserId}, User='{App.CurrentUser.Username}'");
            }
            else
            {
                // Keep existing UserId for updates
                Console.WriteLine($"DEBUG: SaveToEntity - Update media - Existing UserId={_mediaItem.UserId}");
            }
        }

        private async void Save(object? obj)
        {
            Console.WriteLine("=== SAVE METHOD STARTED ===");
            Console.WriteLine($"Current User: {(App.CurrentUser == null ? "NULL" : $"Id={App.CurrentUser.Id}, Username={App.CurrentUser.Username}")}");

            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Title cannot be empty!");
                return;
            }

            try
            {
                // Debug: Check database before save
                Console.WriteLine("=== BEFORE SAVE DEBUG ===");

                // Get database context to check users
                using var scope = App.Services?.CreateScope();
                if (scope != null)
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var users = dbContext.Users.ToList();
                    Console.WriteLine($"Users in database: {users.Count}");
                    foreach (var user in users)
                    {
                        Console.WriteLine($"  User Id: {user.Id}, Username: {user.Username}");
                    }
                }

                SaveToEntity();

                Console.WriteLine($"=== AFTER SaveToEntity ===");
                Console.WriteLine($"Media UserId: {_mediaItem.UserId}");
                Console.WriteLine($"Media Title: {_mediaItem.Title}");
                Console.WriteLine($"Is New: {_isNew}");

                if (_isNew)
                {
                    Console.WriteLine($"Calling MediaService.AddAsync for '{_mediaItem.Title}'...");
                    await _mediaService.AddAsync(_mediaItem);
                    Console.WriteLine($"AddAsync completed successfully!");
                }
                else
                {
                    Console.WriteLine($"Calling MediaService.UpdateAsync for '{_mediaItem.Title}'...");
                    await _mediaService.UpdateAsync(_mediaItem);
                    Console.WriteLine($"UpdateAsync completed successfully!");
                }

                CloseWindow(true);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Save failed: {ex.Message}";
                Console.WriteLine($"ERROR in Save: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // Show inner exception details
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    errorMessage += $"\n\nInner: {ex.InnerException.Message}";

                    // Add more detailed SQLite error info
                    if (ex.InnerException.Message.Contains("FOREIGN KEY"))
                    {
                        errorMessage += "\n\nFOREIGN KEY DETAILS:";
                        errorMessage += $"\n- Trying to insert Media with UserId: {_mediaItem.UserId}";
                        errorMessage += "\n- Checking if user exists...";

                        // Check if user exists
                        try
                        {
                            using var scope = App.Services?.CreateScope();
                            if (scope != null)
                            {
                                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                                var userExists = dbContext.Users.Any(u => u.Id == _mediaItem.UserId);
                                errorMessage += $"\n- User with Id={_mediaItem.UserId} exists: {userExists}";

                                if (!userExists)
                                {
                                    var allUsers = dbContext.Users.Select(u => u.Id).ToList();
                                    errorMessage += $"\n- Available UserIds: {string.Join(", ", allUsers)}";
                                }
                            }
                        }
                        catch (Exception checkEx)
                        {
                            errorMessage += $"\n- Error checking users: {checkEx.Message}";
                        }
                    }
                }

                MessageBox.Show(errorMessage, "Save Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object? obj)
        {
            CloseWindow(false);
        }

        private void AddTag(object? obj)
        {
            if (!string.IsNullOrWhiteSpace(NewTag) && !Tags.Contains(NewTag))
            {
                Tags.Add(NewTag);
                NewTag = "";
                OnPropertyChanged(nameof(NewTag));
            }
        }

        private void RemoveTag(object? obj)
        {
            if (obj is string tag)
            {
                Tags.Remove(tag);
            }
        }

        private void BrowseImage(object? obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                Title = "Select cover image"
            };

            if (dialog.ShowDialog() == true)
            {
                CoverImagePath = dialog.FileName;
                OnPropertyChanged(nameof(CoverImagePath));
            }
        }

        private void CloseWindow(bool dialogResult)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = dialogResult;
                    window.Close();
                    break;
                }
            }
        }

        // IDataErrorInfo implementation
        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Title) && string.IsNullOrWhiteSpace(Title))
                    return "Title cannot be empty";

                if (columnName == nameof(Year) && Year.HasValue && (Year < 1800 || Year > DateTime.Now.Year + 10))
                    return $"Year must be between 1800 and {DateTime.Now.Year + 10}";

                if (columnName == nameof(Rating) && Rating.HasValue && (Rating < 0 || Rating > 10))
                    return "Rating must be between 0 and 10";

                return "";
            }
        }
    }
}
