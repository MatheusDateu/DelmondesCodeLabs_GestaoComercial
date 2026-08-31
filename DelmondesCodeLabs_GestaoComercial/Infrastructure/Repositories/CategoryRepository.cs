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
    #region Category Repository
    /// <summary>
    /// Implements the write operations for the category domain using Entity Framework Core.
    /// </summary>
    public class CategoryRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ICategoryRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new category to the underlying database context.
        /// </summary>
        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Categories.AddAsync(category, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing category's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Categories.Update(category);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a category for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Categories.Remove(category);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion
    }
    #endregion
}