namespace DelmondesCodeLabs_GestaoComercial.Domain.DTOs
{
    #region Financial Summary DTO
    /// <summary>
    /// Represents the aggregated financial metrics for a specific period, used in the BI dashboard.
    /// </summary>
    public class FinancialSummaryDto
    {
        #region Properties
        /// <summary>
        /// Gets or sets the total gross revenue from completed sales.
        /// </summary>
        public decimal GrossRevenue { get; set; }

        /// <summary>
        /// Gets or sets the total value of discounts applied to completed sales.
        /// </summary>
        public decimal TotalDiscounts { get; set; }

        /// <summary>
        /// Gets or sets the total Cost of Goods Sold (COGS) based on the unit cost at the time of sale.
        /// </summary>
        public decimal CostOfGoodsSold { get; set; }

        /// <summary>
        /// Gets or sets the total operational expenses paid within the period.
        /// </summary>
        public decimal TotalExpenses { get; set; }

        /// <summary>
        /// Gets or sets the total financial loss from damaged items.
        /// </summary>
        public decimal TotalLosses { get; set; }

        /// <summary>
        /// Gets the calculated net profit (Gross Revenue - Discounts - COGS - Expenses).
        /// </summary>
        public decimal NetProfit => GrossRevenue - TotalDiscounts - CostOfGoodsSold - TotalExpenses;
        #endregion Properties
    }
    #endregion Financial Summary DTO
}