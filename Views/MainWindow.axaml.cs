// MainWindow.axaml.cs
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
// The using for ViewModel is still needed for x:DataType in XAML, but not for DataContext assignment here.
// using SpendWise.ViewModels;

namespace SpendWise.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // IMPORTANT: Removed DataContext assignment here, as it's now handled in App.axaml.cs
            // DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
