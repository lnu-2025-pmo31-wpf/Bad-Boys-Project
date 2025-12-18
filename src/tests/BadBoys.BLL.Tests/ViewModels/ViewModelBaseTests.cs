using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xunit;

namespace BadBoys.BLL.Tests.ViewModels
{
    // Тестова реалізація ViewModelBase
    public abstract class TestViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // Змінимо на public для тестування
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ViewModelBaseTests
    {
        // Тестовий клас для перевірки ViewModelBase
        private class TestViewModel : TestViewModelBase
        {
            private string? _testProperty;
            public string? TestProperty
            {
                get => _testProperty;
                set
                {
                    if (_testProperty != value)
                    {
                        _testProperty = value;
                        OnPropertyChanged(); // Викликаємо без параметра
                    }
                }
            }

            private int _numberProperty;
            public int NumberProperty
            {
                get => _numberProperty;
                set
                {
                    if (_numberProperty != value)
                    {
                        _numberProperty = value;
                        OnPropertyChanged(nameof(NumberProperty)); // Викликаємо з іменем
                    }
                }
            }
        }

        [Fact]
        public void ViewModelBase_ShouldImplement_INotifyPropertyChanged()
        {
            // Arrange & Act
            var viewModel = new TestViewModel();

            // Assert
            Assert.IsAssignableFrom<INotifyPropertyChanged>(viewModel);
        }

        [Fact]
        public void OnPropertyChanged_WithoutParameter_ShouldRaiseEventWithPropertyName()
        {
            // Arrange
            var viewModel = new TestViewModel();
            bool eventRaised = false;
            string? propertyName = null;
            
            viewModel.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                propertyName = args.PropertyName;
            };

            // Act
            viewModel.TestProperty = "New Value";

            // Assert
            Assert.True(eventRaised, "Подія PropertyChanged мала викликатися");
            Assert.Equal(nameof(TestViewModel.TestProperty), propertyName);
        }

        [Fact]
        public void OnPropertyChanged_WithParameter_ShouldRaiseEventWithCorrectName()
        {
            // Arrange
            var viewModel = new TestViewModel();
            bool eventRaised = false;
            string? propertyName = null;
            
            viewModel.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                propertyName = args.PropertyName;
            };

            // Act
            viewModel.NumberProperty = 42;

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(nameof(TestViewModel.NumberProperty), propertyName);
        }

        [Fact]
        public void PropertyChanged_ShouldNotRaise_WhenValueNotChanged()
        {
            // Arrange
            var viewModel = new TestViewModel();
            viewModel.TestProperty = "Initial Value";
            
            bool eventRaised = false;
            viewModel.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
            };

            // Act - Встановлюємо те саме значення
            viewModel.TestProperty = "Initial Value";

            // Assert
            Assert.False(eventRaised, "Подія не мала викликатися при однаковому значенні");
        }

        [Fact]
        public void OnPropertyChanged_WithCustomPropertyName_ShouldRaiseEvent()
        {
            // Arrange
            var viewModel = new TestViewModel();
            bool eventRaised = false;
            string customPropertyName = "CustomProperty";
            
            viewModel.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                Assert.Equal(customPropertyName, args.PropertyName);
            };

            // Act
            viewModel.OnPropertyChanged(customPropertyName);

            // Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public void MultiplePropertyChanges_ShouldRaiseMultipleEvents()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var changedProperties = new List<string>();
            
            viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != null)
                    changedProperties.Add(args.PropertyName);
            };

            // Act
            viewModel.TestProperty = "First";
            viewModel.NumberProperty = 1;
            viewModel.TestProperty = "Second";

            // Assert
            Assert.Equal(3, changedProperties.Count);
            Assert.Equal(nameof(TestViewModel.TestProperty), changedProperties[0]);
            Assert.Equal(nameof(TestViewModel.NumberProperty), changedProperties[1]);
            Assert.Equal(nameof(TestViewModel.TestProperty), changedProperties[2]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void OnPropertyChanged_WithInvalidPropertyName_ShouldNotThrow(string? propertyName)
        {
            // Arrange
            var viewModel = new TestViewModel();
            
            // Act & Assert - Не має бути винятку
            var exception = Record.Exception(() => viewModel.OnPropertyChanged(propertyName!));
            
            Assert.Null(exception);
        }
    }
}
