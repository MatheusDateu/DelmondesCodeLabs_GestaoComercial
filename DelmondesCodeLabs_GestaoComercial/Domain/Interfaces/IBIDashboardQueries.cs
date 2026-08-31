#region Usings
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DelmondesCodeLabs_GestaoComercial.Domain.DTOs;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Interfaces
{
    #region BI Dashboard Queries Interface
    /// <summary>
    /// Defines the contract for high-performance Business Intelligence and reporting queries.
    /// </summary>
    public interface IBIDashboardQueries
    {
        #region Methods
        /// <summary>
        /// Retrieves the aggregated financial summary for a specific date range.
        /// </summary>
        Task<FinancialSummaryDto> GetFinancialSummaryAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the top-performing products based on sales revenue for the ABC curve analysis.
        /// </summary>
        Task<IEnumerable<AbcProductDto>> GetAbcSalesCurveAsync(DateTimeOffset startDate, DateTimeOffset endDate, int topN = 10, CancellationToken cancellationToken = default);
        #endregion Methods
    }
    #endregion BI Dashboard Queries Interface
}