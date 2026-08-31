#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Promotion Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for discount promotions using Dapper.
    /// </summary>
    public interface IPromotionQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a promotion by its unique identifier, including its associated items.
        /// </summary>
        Task<Promotion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of promotions, optionally filtered by a search term.
        /// </summary>
        Task<PagedResult<Promotion>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all currently active and valid promotions.
        /// </summary>
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync(CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Promotion Queries Interface
}