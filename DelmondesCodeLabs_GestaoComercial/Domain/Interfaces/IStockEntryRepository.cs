#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for stock entry entities using Entity Framework Core.
    /// </summary>
    public interface IStockEntryRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new stock entry and its associated items to the database context.
        /// </summary>
        Task AddAsync(StockEntry stockEntry, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing stock entry's state in the database context.
        /// </summary>
        Task UpdateAsync(StockEntry stockEntry, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a stock entry for soft deletion.
        /// </summary>
        Task DeleteAsync(StockEntry stockEntry, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}