#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Stock Movement Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for the stock movement ledger using Dapper.
    /// </summary>
    public interface IStockMovementQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a paginated history of stock movements for a specific product.
        /// </summary>
        Task<PagedResult<StockMovement>> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Stock Movement Queries Interface
}