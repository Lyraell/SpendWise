// AddExpenseWindow.axaml.cs
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SpendWise.Models; // Updated namespace
using SpendWise.ViewModels; // Updated namespace
using System;

namespace SpendWise.Views // Updated namespace
{
    public partial class AddExpenseWindow : Window
    {
        public AddExpenseWindow()
        {
            InitializeComponent();
            DataContext = new AddExpenseWindowViewModel();

            if (DataContext is AddExpenseWindowViewModel viewModel)
            {
                viewModel.CloseWindow += (sender, expense) =>
                {
                    Close(expense);
                };
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}