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
    #region Supplier Repository
    /// <summary>
    /// Implements the write operations for the supplier domain using Entity Framework Core.
    /// </summary>
    public class SupplierRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ISupplierRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new supplier to the underlying database context.
        /// </summary>
        public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Suppliers.AddAsync(supplier, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing supplier's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Suppliers.Update(supplier);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a supplier for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Suppliers.Remove(supplier);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Supplier Repository
}