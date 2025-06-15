// ViewModels/MainWindowViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using SpendWise.Views;
using System.Linq; // For Sum()
using SpendWise.Models; // For Expense model
using Avalonia.Threading; // Crucial for Dispatcher.UIThread
using System.Reactive.Linq; // Needed for ObserveOn extension method (though we'll use a ctor parameter)
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes; // For IClassicDesktopStyleApplicationLifetime
namespace SpendWise.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Added '?' to allow null for Description, or initialized in ctor
        private ObservableCollection<Expense> _expenses;
        private double _currentMonthTotal;
        private string _currentMonthTotalFormatted;
        private bool _noExpensesAdded;
        private bool _hasExpenses;

        public MainWindowViewModel()
        {
            // Initialize collections and properties to non-null values
            _expenses = new ObservableCollection<Expense>(); // Initialized here
            _currentMonthTotalFormatted = "0.00"; // Initialized here
            UpdateExpenseSummary(); // Initial calculation and UI update

            // Define the command to add a new expense
            // IMPORTANT FIX: Pass RxApp.MainThreadScheduler directly to CreateFromTask
            // This handles the CanExecute/IsEnabled updates on the UI thread without breaking the ReactiveCommand type.
           

            // Subscribe to the CollectionChanged event of Expenses.
            Expenses.CollectionChanged += (sender, e) => UpdateExpenseSummary();
        }

        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;
            set => this.RaiseAndSetIfChanged(ref _expenses, value);
        }

        public double CurrentMonthTotal
        {
            get => _currentMonthTotal;
            set => this.RaiseAndSetIfChanged(ref _currentMonthTotal, value);
        }

        public string CurrentMonthTotalFormatted
        {
            get => _currentMonthTotalFormatted;
            set => this.RaiseAndSetIfChanged(ref _currentMonthTotalFormatted, value);
        }

        public bool NoExpensesAdded
        {
            get => _noExpensesAdded;
            set => this.RaiseAndSetIfChanged(ref _noExpensesAdded, value);
        }

        public bool HasExpenses
        {
            get => _hasExpenses;
            set => this.RaiseAndSetIfChanged(ref _hasExpenses, value);
        }
        public async void AddNewExpenseCommand()
        {
            var addExpenseWindow = new AddExpenseWindow();
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
            if (mainWindow == null)
            {
                Console.WriteLine("Main window is null");
                return;
            }
            var result = await addExpenseWindow.ShowDialog<Expense>(mainWindow);
            if (result != null)
            {
                // Add the new expense to the collection
                Expenses.Add(result);
                UpdateExpenseSummary(); // Update the summary after adding a new expense
            }
            else
            {
                Console.WriteLine("No expense was added.");
            }
        }

        private void UpdateExpenseSummary()
        {
            var now = DateTime.Now;
            CurrentMonthTotal = Expenses.Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
                                        .Sum(e => e.Amount);
            CurrentMonthTotalFormatted = CurrentMonthTotal.ToString("F2");

            NoExpensesAdded = Expenses.Count == 0;
            HasExpenses = Expenses.Count > 0;
        }
    }
}
