using DelmondesCodeLabs_GestaoComercial.Domain.Entities;

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for product entities using Entity Framework Core.
    /// </summary>
    public interface IProductRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new product to the underlying database context.
        /// </summary>
        Task AddAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing product's state in the database context.
        /// </summary>
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a product for soft deletion.
        /// </summary>
        Task DeleteAsync(Product product, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion
}
