#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Category Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for categories using Dapper.
    /// </summary>
    public interface ICategoryQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all active categories, typically used to populate POS filter buttons.
        /// </summary>
        Task<IEnumerable<Category>> GetAllActiveAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of categories for the administration dashboard.
        /// </summary>
        Task<PagedResult<Category>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Category Queries Interface
}