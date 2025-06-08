
using System;

namespace SpendWise.Models // Updated namespace
{
    public class Expense
    {
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
