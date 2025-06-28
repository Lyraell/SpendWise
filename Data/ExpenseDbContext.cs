// Data/ExpenseDbContext.cs 
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using SpendWise.Models;
using System.Collections.Generic;

namespace SpendWise.Data
{
    public class ExpenseDbContext : DbContext
    {
       
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }

    }
}