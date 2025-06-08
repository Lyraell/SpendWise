// ViewModels/AddExpenseWindowViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using SpendWise.Models; // Updated namespace

namespace SpendWise.ViewModels // Updated namespace
{
    public class AddExpenseWindowViewModel : ViewModelBase
    {
        private string _selectedCategory;
        private string _amount;
        private DateTimeOffset _selectedDate;

        public event EventHandler<Expense> CloseWindow;

        public AddExpenseWindowViewModel()
        {
            ExpenseCategories = new ObservableCollection<string>
            {
                "Food", "Leisure", "Utilities", "Transport", "Shopping", "Rent", "Health", "Education", "Other"
            };

            SelectedDate = DateTimeOffset.Now;

            AddExpenseCommand = ReactiveCommand.Create(AddExpense);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        public ObservableCollection<string> ExpenseCategories { get; }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
        }

        public string Amount
        {
            get => _amount;
            set => this.RaiseAndSetIfChanged(ref _amount, value);
        }

        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set => this.RaiseAndSetIfChanged(ref _selectedDate, value);
        }

        public ReactiveCommand<Unit, Unit> AddExpenseCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        private void AddExpense()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                Console.WriteLine("Please select a category.");
                CloseWindow?.Invoke(this, null);
                return;
            }

            if (!double.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out double amountValue) || amountValue <= 0)
            {
                Console.WriteLine("Please enter a valid positive amount.");
                CloseWindow?.Invoke(this, null);
                return;
            }

            var newExpense = new Expense
            {
                Description = SelectedCategory,
                Amount = amountValue,
                Date = SelectedDate.Date
            };

            CloseWindow?.Invoke(this, newExpense);
        }

        private void Cancel()
        {
            CloseWindow?.Invoke(this, null);
        }
    }
}