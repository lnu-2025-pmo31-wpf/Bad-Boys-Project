using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BadBoys.Presentation.WPF.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private readonly Action<Exception>? _errorHandler;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null, Action<Exception>? errorHandler = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public RelayCommand(Func<object?, Task> executeAsync, Func<object?, bool>? canExecute = null, Action<Exception>? errorHandler = null)
        {
            _execute = async param =>
            {
                try
                {
                    await executeAsync(param);
                }
                catch (Exception ex)
                {
                    _errorHandler?.Invoke(ex);
                    // Default error handling if none provided
                    if (_errorHandler == null)
                    {
                        System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            };
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
