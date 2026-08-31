#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for discount promotions using Entity Framework Core.
    /// </summary>
    public interface IPromotionRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new promotion and its items to the underlying database context.
        /// </summary>
        Task AddAsync(Promotion promotion, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing promotion's state in the database context.
        /// </summary>
        Task UpdateAsync(Promotion promotion, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a promotion for soft deletion.
        /// </summary>
        Task DeleteAsync(Promotion promotion, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}