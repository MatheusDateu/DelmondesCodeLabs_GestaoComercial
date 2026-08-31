#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for supplier entities using Entity Framework Core.
    /// </summary>
    public interface ISupplierRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new supplier to the underlying database context.
        /// </summary>
        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing supplier's state in the database context.
        /// </summary>
        Task UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a supplier for soft deletion.
        /// </summary>
        Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion
}