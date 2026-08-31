#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Supplier Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for suppliers using Dapper.
    /// </summary>
    public interface ISupplierQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a supplier by its unique identifier.
        /// </summary>
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of suppliers for the administration dashboard.
        /// </summary>
        Task<PagedResult<Supplier>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Supplier Queries Interface
}