#region Usings
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Repositories
{
    #region Stock Entry Repository
    /// <summary>
    /// Implements the write operations for the stock entry domain using Entity Framework Core.
    /// </summary>
    public class StockEntryRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IStockEntryRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new stock entry and its associated items to the database context.
        /// </summary>
        public async Task AddAsync(StockEntry stockEntry, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.StockEntries.AddAsync(stockEntry, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing stock entry's state in the database context.
        /// </summary>
        public async Task UpdateAsync(StockEntry stockEntry, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.StockEntries.Update(stockEntry);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a stock entry for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(StockEntry stockEntry, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.StockEntries.Remove(stockEntry);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
    #endregion
}