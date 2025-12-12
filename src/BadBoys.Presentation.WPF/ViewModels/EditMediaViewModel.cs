using BadBoys.Presentation.WPF.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace BadBoys.Presentation.WPF.ViewModels
{
    public class EditMediaViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";

        public ICommand SaveCommand { get; }

        public EditMediaViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object? obj)
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Category))
                return;

            // TODO: Save to BLL
        }

        public string Error => "";

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Title) && string.IsNullOrWhiteSpace(Title))
                    return "Назва не може бути пустою";

                if (columnName == nameof(Category) && string.IsNullOrWhiteSpace(Category))
                    return "Категорія не може бути пустою";

                return "";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
