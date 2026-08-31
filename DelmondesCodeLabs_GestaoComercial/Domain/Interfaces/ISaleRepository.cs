#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for sales transactions using Entity Framework Core.
    /// </summary>
    public interface ISaleRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new sale and its items to the underlying database context.
        /// </summary>
        Task AddAsync(Sale sale, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing sale's state in the database context.
        /// </summary>
        Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a sale for soft deletion or cancellation.
        /// </summary>
        Task DeleteAsync(Sale sale, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}