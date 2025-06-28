// Data/ExpenseDbContextFactory.cs (New File)
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SpendWise.Data
{
    public class ExpenseDbContextFactory : IDesignTimeDbContextFactory<ExpenseDbContext>
    {
        public ExpenseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExpenseDbContext>();

            // The same connection string you had in OnConfiguring
            const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=SpendWiseDB;Trusted_Connection=True;TrustServerCertificate=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new ExpenseDbContext(optionsBuilder.Options);
        }
    }
}