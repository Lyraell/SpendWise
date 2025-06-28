// ViewModels/MainWindowViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using SpendWise.Models;
using SpendWise.Services;
using SpendWise.Views;

namespace SpendWise.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ExpenseService _expenseService;
        private ObservableCollection<Expense> _allExpenses;
        private ObservableCollection<Expense> _displayedExpenses;
        private DateTime _displayedMonth;
        private string _displayedMonthText = string.Empty;
        private string _currentMonthTotalFormatted = "0.00";
        private bool _isAnyExpenseAdded;
        private bool _hasExpensesForCurrentMonth;

        public MainWindowViewModel()
        {
            _expenseService = new ExpenseService();
            _expenseService.EnsureDatabaseCreated();
            _allExpenses = new ObservableCollection<Expense>();
            _displayedExpenses = new ObservableCollection<Expense>();
            DisplayedMonth = DateTime.Now;


            _ = LoadAllExpensesAsync();
        }

        // Collections
        public ObservableCollection<Expense> DisplayedExpenses
        {
            get => _displayedExpenses;
            set => this.RaiseAndSetIfChanged(ref _displayedExpenses, value);
        }

        // Properties for UI Binding
        public DateTime DisplayedMonth
        {
            get => _displayedMonth;
            set
            {
                this.RaiseAndSetIfChanged(ref _displayedMonth, value);
                // Display the month and year, e.g., "June 2025"
                DisplayedMonthText = value.ToString("MMMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        public string DisplayedMonthText
        {
            get => _displayedMonthText;
            set => this.RaiseAndSetIfChanged(ref _displayedMonthText, value);
        }

        public string CurrentMonthTotalFormatted
        {
            get => _currentMonthTotalFormatted;
            set => this.RaiseAndSetIfChanged(ref _currentMonthTotalFormatted, value);
        }

        public bool IsAnyExpenseAdded
        {
            get => _isAnyExpenseAdded;
            set => this.RaiseAndSetIfChanged(ref _isAnyExpenseAdded, value);
        }

        public bool HasExpensesForCurrentMonth
        {
            get => _hasExpensesForCurrentMonth;
            set => this.RaiseAndSetIfChanged(ref _hasExpensesForCurrentMonth, value);
        }

        
        // Core Methods
        private async Task LoadAllExpensesAsync()
        {
            var loadedExpenses = await _expenseService.LoadExpensesAsync();
            _allExpenses = new ObservableCollection<Expense>(loadedExpenses);
            UpdateViewForCurrentMonth();
        }

        public async void AddNewExpenseCommand()
        {
            var addExpenseWindow = new AddExpenseWindow();
            var mainWindow = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;

            if (mainWindow != null)
            {
                var result = await addExpenseWindow.ShowDialog<Expense>(mainWindow);
                if (result != null)
                {
                    await _expenseService.AddExpenseAsync(result);
                    _allExpenses.Add(result);
                    UpdateViewForCurrentMonth();
                }
            }
        }

        private void UpdateViewForCurrentMonth()
        {
            var expensesForMonth = _allExpenses
                .Where(e => e.Date.Year == DisplayedMonth.Year && e.Date.Month == DisplayedMonth.Month)
                .OrderByDescending(e => e.Date)
                .ToList();

            DisplayedExpenses = new ObservableCollection<Expense>(expensesForMonth);

            var total = expensesForMonth.Sum(e => e.Amount);
            CurrentMonthTotalFormatted = total.ToString("F2", CultureInfo.InvariantCulture);

            IsAnyExpenseAdded = _allExpenses.Any();
            HasExpensesForCurrentMonth = DisplayedExpenses.Any();
        }

        public async void NextMonthCommand()
        {
            DisplayedMonth = DisplayedMonth.AddMonths(1);
            UpdateViewForCurrentMonth();
        }

        public async void PreviousMonthCommand()
        {
            DisplayedMonth = DisplayedMonth.AddMonths(-1);
            UpdateViewForCurrentMonth();
        }
    }
}