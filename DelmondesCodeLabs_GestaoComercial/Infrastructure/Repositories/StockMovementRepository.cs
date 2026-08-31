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
    #region Stock Movement Repository
    /// <summary>
    /// Implements the write operations for the stock movement ledger using Entity Framework Core.
    /// </summary>
    public class StockMovementRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IStockMovementRepository
    {
        #region Methods
        /// <summary>
        /// Appends a new stock movement record to the database context.
        /// </summary>
        public async Task AddAsync(StockMovement stockMovement, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.StockMovements.AddAsync(stockMovement, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a stock movement for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(StockMovement stockMovement, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.StockMovements.Remove(stockMovement);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Stock Movement Repository
}