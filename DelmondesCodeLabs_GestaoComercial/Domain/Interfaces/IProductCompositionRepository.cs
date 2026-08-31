#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for product compositions (Bill of Materials).
    /// </summary>
    public interface IProductCompositionRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new product composition link to the underlying database context.
        /// </summary>
        Task AddAsync(ProductComposition productComposition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the quantity or details of an existing product composition link.
        /// </summary>
        Task UpdateAsync(ProductComposition productComposition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a product composition link for soft deletion.
        /// </summary>
        Task DeleteAsync(ProductComposition productComposition, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}