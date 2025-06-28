// Services/ExpenseService.cs (Corrected)
using Microsoft.EntityFrameworkCore; // Added this line
using SpendWise.Data;
using SpendWise.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpendWise.Services
{
    public class ExpenseService
    {
        // This is a factory method to create the DbContext instance
        private ExpenseDbContext CreateDbContext()
        {
            var factory = new ExpenseDbContextFactory();
            return factory.CreateDbContext(new string[0]);
        }

        public async Task<List<Expense>> LoadExpensesAsync()
        {
            // Create a new context for this operation
            using var context = CreateDbContext();
            return await context.Expenses.ToListAsync();
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            // Create a new context for this operation
            using var context = CreateDbContext();
            context.Expenses.Add(expense);
            await context.SaveChangesAsync();
        }

        public void EnsureDatabaseCreated()
        {
            // Create a context just to run the migration
            using var context = CreateDbContext();
            // This line now works because of the 'using' statement
            context.Database.Migrate();
        }
    }
}