#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Data;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
using DelmondesCodeLabs_GestaoComercial.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Infrastructure.Repositories
{
    #region Product Repository
    /// <summary>
    /// Implements the write operations for the product domain using Entity Framework Core with DbContextFactory for Blazor Server compatibility.
    /// </summary>
    public class ProductRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IProductRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new product to the underlying database context.
        /// </summary>
        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Products.AddAsync(product, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing product's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Products.Update(product);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a product for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Products.Remove(product);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Product Repository
}