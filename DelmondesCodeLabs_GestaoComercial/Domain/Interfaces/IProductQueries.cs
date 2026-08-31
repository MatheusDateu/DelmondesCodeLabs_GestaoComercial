#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Read Queries Interface
    /// <summary>
    /// Defines the high-performance read operations (Queries) for products using Dapper.
    /// </summary>
    public interface IProductQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a product by its unique stock keeping unit (SKU) or barcode.
        /// Useful for POS barcode scanners.
        /// </summary>
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of products, optionally filtered by a full-text search term.
        /// </summary>
        Task<PagedResult<Product>> SearchProductsAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Read Queries Interface
}