// Models/Expense.cs
using System;

namespace SpendWise.Models
{
    public class Expense
    {
        // Initialized string properties to empty string to avoid CS8618 warning
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
