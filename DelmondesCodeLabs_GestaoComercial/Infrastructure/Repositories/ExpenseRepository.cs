#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Repositories
{
    #region Expense Repository
    /// <summary>
    /// Implements the write operations for the expense domain using Entity Framework Core with DbContextFactory.
    /// </summary>
    public class ExpenseRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IExpenseRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new expense record to the underlying database context.
        /// </summary>
        public async Task AddAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Expenses.AddAsync(expense, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing expense's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Expenses.Update(expense);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks an expense for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Expense expense, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Expenses.Remove(expense);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Expense Repository
}