// ViewModels/AddExpenseWindowViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using SpendWise.Models;

namespace SpendWise.ViewModels
{
    public class AddExpenseWindowViewModel : ViewModelBase
    {
        
        private string? _selectedCategory;
        private string _amount = string.Empty;
        private DateTimeOffset _selectedDate;

        public event EventHandler<Expense?>? CloseWindow; // Expense can be null if cancelled

        public AddExpenseWindowViewModel()
        {
            ExpenseCategories = new ObservableCollection<string>
            {
                "Food", "Leisure", "Utilities", "Transport", "Shopping", "Rent", "Health", "Education", "Other"
            };

            SelectedDate = DateTimeOffset.Now; // Default to today's date

            
        }

        public ObservableCollection<string> ExpenseCategories { get; }

        public string? SelectedCategory // Declared as nullable property
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


        public void AddExpenseCommand()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                Console.WriteLine("Please select a category.");
                CloseWindow?.Invoke(this, null); // Pass null explicitly for nullable Expense?
                return;
            }

            if (!double.TryParse(Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out double amountValue) || amountValue <= 0)
            {
                Console.WriteLine("Please enter a valid positive amount.");
                CloseWindow?.Invoke(this, null); // Pass null explicitly for nullable Expense?
                return;
            }

            var newExpense = new Expense
            {
                Description = SelectedCategory, // SelectedCategory is non-null here due to check above
                Amount = amountValue,
                Date = SelectedDate.Date
            };

            CloseWindow?.Invoke(this, newExpense);
        }

        public void CancelCommand()
        {
            CloseWindow?.Invoke(this, null); // Pass null explicitly for nullable Expense?
        }
    }
}
