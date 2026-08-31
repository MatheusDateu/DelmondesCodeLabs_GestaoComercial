#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Stock Entry Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for stock entries and invoices using Dapper.
    /// </summary>
    public interface IStockEntryQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a stock entry by its unique identifier, including its line items and supplier details.
        /// </summary>
        Task<StockEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of stock entries for the administration dashboard.
        /// </summary>
        Task<PagedResult<StockEntry>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Stock Entry Queries Interface
}