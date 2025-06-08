// App.axaml.cs
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using SpendWise.ViewModels;
using SpendWise.Views;
using Avalonia.Threading; // Added for Dispatcher
using System; // Added for Exception

namespace SpendWise
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                // Setting MainWindow and its DataContext directly here, as per your structure
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                // *** GLOBAL EXCEPTION HANDLER FOR UI THREAD ***
                // This will catch any unhandled exceptions occurring on Avalonia's main UI thread.
                Dispatcher.UIThread.UnhandledException += (sender, e) =>
                {
                    Console.WriteLine($"\n*** UNHANDLED UI THREAD EXCEPTION ***");
                    Console.WriteLine($"Exception Type: {e.Exception.GetType().Name}");
                    Console.WriteLine($"Message: {e.Exception.Message}");
                    Console.WriteLine($"Stack Trace:\n{e.Exception.StackTrace}");
                    Console.WriteLine($"Source: UIThread");
                    Console.WriteLine($"*** END UNHANDLED EXCEPTION ***\n");

                    // Set e.Handled to true to prevent the application from crashing immediately for debugging.
                    // In a production app, you might want to log this and/or show a user-friendly dialog
                    // before deciding whether to allow the app to terminate.
                    e.Handled = true;
                };

                // *** GLOBAL EXCEPTION HANDLER FOR UNHANDLED TASK EXCEPTIONS ***
                // This catches exceptions from tasks that are not explicitly awaited (fire-and-forget tasks).
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    var ex = e.ExceptionObject as Exception;
                    if (ex != null)
                    {
                        Console.WriteLine($"\n*** UNHANDLED APP DOMAIN EXCEPTION (Possibly from Task) ***");
                        Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                        Console.WriteLine($"Message: {ex.Message}");
                        Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                        Console.WriteLine($"IsTerminating: {e.IsTerminating}"); // Indicates if the runtime is exiting
                        Console.WriteLine($"*** END UNHANDLED EXCEPTION ***\n");
                    }
                    // Note: If e.IsTerminating is true, the application process will typically exit anyway.
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}
