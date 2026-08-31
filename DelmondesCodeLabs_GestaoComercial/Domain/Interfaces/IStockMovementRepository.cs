#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for the immutable stock movement ledger.
    /// </summary>
    public interface IStockMovementRepository
    {
        #region Methods
        /// <summary>
        /// Appends a new stock movement record to the database context.
        /// </summary>
        Task AddAsync(StockMovement stockMovement, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a stock movement for soft deletion. 
        /// </summary>
        Task DeleteAsync(StockMovement stockMovement, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}