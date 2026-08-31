#region Usings
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Write Repository Interface
    /// <summary>
    /// Defines the write operations (Commands) for operational expenses using Entity Framework Core.
    /// </summary>
    public interface IExpenseRepository
    {
        #region Methods
        /// <summary>
        /// Adds a new expense record to the underlying database context.
        /// </summary>
        Task AddAsync(Expense expense, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing expense's state in the database context.
        /// </summary>
        Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an expense for soft deletion.
        /// </summary>
        Task DeleteAsync(Expense expense, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Write Repository Interface
}