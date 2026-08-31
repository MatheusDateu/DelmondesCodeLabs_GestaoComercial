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
    #region Product Composition Repository
    /// <summary>
    /// Implements the write operations for the product composition domain using Entity Framework Core.
    /// </summary>
    public class ProductCompositionRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IProductCompositionRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new product composition link to the underlying database context.
        /// </summary>
        public async Task AddAsync(ProductComposition productComposition, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.ProductCompositions.AddAsync(productComposition, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates the quantity or details of an existing product composition link.
        /// </summary>
        public async Task UpdateAsync(ProductComposition productComposition, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.ProductCompositions.Update(productComposition);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a product composition link for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(ProductComposition productComposition, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.ProductCompositions.Remove(productComposition);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
    #endregion
}