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
    #region Sale Repository
    /// <summary>
    /// Implements the write operations for the sales domain using Entity Framework Core with DbContextFactory.
    /// </summary>
    public class SaleRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ISaleRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new sale and its items to the underlying database context.
        /// </summary>
        public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            foreach (SaleItem item in sale.Items)
            {
                if (item.Product != null)
                {
                    context.Entry(item.Product).State = EntityState.Unchanged;
                }
            }

            await context.Sales.AddAsync(sale, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing sale's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);

            foreach (SaleItem item in sale.Items)
            {
                if (item.Product != null)
                {
                    context.Entry(item.Product).State = EntityState.Unchanged;
                }
            }

            context.Sales.Update(sale);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a sale for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Sales.Remove(sale);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Sale Repository
}