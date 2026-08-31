#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Sale Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for sales transactions using Dapper.
    /// </summary>
    public interface ISaleQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a sale by its unique identifier, including all its line items and products.
        /// </summary>
        Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of sales, optionally filtered by receipt number or date.
        /// </summary>
        Task<PagedResult<Sale>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all currently open tabs (pending sales), ordered by the most recently modified.
        /// </summary>
        Task<IEnumerable<Sale>> GetOpenTabsAsync(string? searchTerm, CancellationToken cancellationToken = default);

        #endregion Methods
    }
    #endregion Sale Queries Interface
}