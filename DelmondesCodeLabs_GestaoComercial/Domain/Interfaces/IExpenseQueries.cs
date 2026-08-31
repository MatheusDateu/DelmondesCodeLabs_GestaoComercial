#region Usings
using System;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using DelmondesCodeLabs_GestaoComercial.Domain.Entities;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region Expense Queries Interface
    /// <summary>
    /// Defines the high-performance read operations for operational expenses using Dapper.
    /// </summary>
    public interface IExpenseQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves a paginated list of expenses, optionally filtered by description or category.
        /// </summary>
        Task<PagedResult<Expense>> GetPaginatedAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion Expense Queries Interface
}