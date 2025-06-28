// Models/Expense.cs
using System;

namespace SpendWise.Models
{
    public class Expense
    {
        public int ID { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
