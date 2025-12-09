using System;
using System.ComponentModel;

namespace BadBoys.Presentation.WPF.ViewModels
{
    // Простий ViewModel з валідацією через IDataErrorInfo
    public class MediaItemViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _title = "";
        private string? _type;
        private double? _rating;
        private string? _description;

        public int Id { get; set; }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(nameof(Title)); }
        }

        public string? Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(nameof(Type)); }
        }

        public double? Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(nameof(Rating)); }
        }

        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Title) when string.IsNullOrWhiteSpace(Title) => "Назва обов'язкова",
                    nameof(Type) when string.IsNullOrWhiteSpace(Type) => "Тип обов'язковий",
                    nameof(Rating) when Rating.HasValue && (Rating < 0 || Rating > 10) => "Рейтинг має бути від 0 до 10",
                    nameof(Description) when !string.IsNullOrEmpty(Description) && Description.Length > 500 => "Опис не більше 500 символів",
                    _ => string.Empty
                };
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
