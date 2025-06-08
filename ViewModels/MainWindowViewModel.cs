using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using SpendWise.Views;
using System.Linq; // For Sum()
using SpendWise.Models; // For Expense model
using Avalonia.Threading; // Crucial for Dispatcher.UIThread

namespace SpendWise.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<Expense> _expenses;
        private double _currentMonthTotal;
        private string _currentMonthTotalFormatted;
        private bool _noExpensesAdded;
        private bool _hasExpenses;

        public MainWindowViewModel()
        {
            // Initialize collections and properties
            Expenses = new ObservableCollection<Expense>();
            CurrentMonthTotalFormatted = "0.00"; // Ensure a default formatted value
            UpdateExpenseSummary(); // Initial calculation and UI update

            // Define the command to add a new expense
            // ReactiveCommand.CreateFromTask allows asynchronous operations.
            // <Window, Unit> means it takes a Window object as input and returns a Unit (void-like result).
            AddNewExpenseCommand = ReactiveCommand.CreateFromTask<Window, Unit>(async (ownerWindow) =>
            {
                // This block is crucial: It ensures that the UI-related operations
                // (creating a new Window and showing it as a dialog) are executed
                // on Avalonia's main UI thread, preventing "InvalidOperationException".
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    var addExpenseWindow = new AddExpenseWindow();
                    // ShowDialog makes the new window modal to the ownerWindow.
                    // It returns the object passed to addExpenseWindow.Close().
                    var result = await addExpenseWindow.ShowDialog<Expense>(ownerWindow);

                    // Check if an expense was returned (i.e., user clicked "Add Expense" and not "Cancel")
                    if (result != null)
                    {
                        Expenses.Add(result); // Add the new expense to the observable collection
                        UpdateExpenseSummary(); // Recalculate and update the total display
                    }
                }); // End of Dispatcher.UIThread.InvokeAsync block

                // Important: Return Unit.Default to satisfy the ReactiveCommand's return type (Unit).
                // This ensures all code paths within the async lambda return a value.
                return Unit.Default;
            });

            // Subscribe to the CollectionChanged event of Expenses.
            // This ensures that UpdateExpenseSummary() is called automatically
            // whenever items are added to or removed from the Expenses collection,
            // keeping the CurrentMonthTotal updated in real-time.
            Expenses.CollectionChanged += (sender, e) => UpdateExpenseSummary();
        }

        // Public property for the collection of expenses, bound to UI later.
        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;
            set => this.RaiseAndSetIfChanged(ref _expenses, value);
        }

        // Public property for the raw total amount, primarily for internal calculations.
        public double CurrentMonthTotal
        {
            get => _currentMonthTotal;
            set => this.RaiseAndSetIfChanged(ref _currentMonthTotal, value);
        }

        // Public property for the formatted total amount, bound directly to the UI TextBlock.
        public string CurrentMonthTotalFormatted
        {
            get => _currentMonthTotalFormatted;
            set => this.RaiseAndSetIfChanged(ref _currentMonthTotalFormatted, value);
        }

        // Boolean flag to control visibility of "No expenses yet" message.
        public bool NoExpensesAdded
        {
            get => _noExpensesAdded;
            set => this.RaiseAndSetIfChanged(ref _noExpensesAdded, value);
        }

        // Boolean flag to control visibility of the actual expenses list (when implemented).
        public bool HasExpenses
        {
            get => _hasExpenses;
            set => this.RaiseAndSetIfChanged(ref _hasExpenses, value);
        }

        // The ReactiveCommand instance that the UI button binds to.
        public ReactiveCommand<Window, Unit> AddNewExpenseCommand { get; }

        // Private method to calculate the current month's total and update UI flags.
        private void UpdateExpenseSummary()
        {
            var now = DateTime.Now;
            // Calculate sum of expenses for the current month and year.
            CurrentMonthTotal = Expenses.Where(e => e.Date.Year == now.Year && e.Date.Month == now.Month)
                                        .Sum(e => e.Amount);
            // Format the calculated total to two decimal places for display.
            CurrentMonthTotalFormatted = CurrentMonthTotal.ToString("F2");

            // Update UI visibility flags based on whether there are any expenses.
            NoExpensesAdded = Expenses.Count == 0;
            HasExpenses = Expenses.Count > 0;
        }
    }
}