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
    #region Promotion Repository
    /// <summary>
    /// Implements the write operations for the promotion domain using Entity Framework Core with DbContextFactory.
    /// </summary>
    public class PromotionRepository(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IPromotionRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new promotion and its items to the underlying database context.
        /// </summary>
        public async Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            await context.Promotions.AddAsync(promotion, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing promotion's state in the database context.
        /// </summary>
        public async Task UpdateAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Promotions.Update(promotion);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Marks a promotion for soft deletion by leveraging the DbContext interceptor.
        /// </summary>
        public async Task DeleteAsync(Promotion promotion, CancellationToken cancellationToken = default)
        {
            await using ApplicationDbContext context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            context.Promotions.Remove(promotion);
            await context.SaveChangesAsync(cancellationToken);
        }
        #endregion Methods
    }
    #endregion Promotion Repository
}