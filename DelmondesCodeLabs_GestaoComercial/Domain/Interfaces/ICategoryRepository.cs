#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for category entities using Entity Framework Core.
    /// </summary>
    public interface ICategoryRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new category to the underlying database context.
        /// </summary>
        Task AddAsync(Category category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing category's state in the database context.
        /// </summary>
        Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a category for soft deletion.
        /// </summary>
        Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
        #endregion
    }
    #endregion
}