#region Usings
using System;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Expense Entity
    /// <summary>
    /// Represents a financial outgoing or operational cost (e.g., rent, utility bills, pool table maintenance).
    /// </summary>
    public class Expense : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets a brief description of the expense.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the financial value of the expense.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the deadline for paying the expense.
        /// </summary>
        public DateTimeOffset DueDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the expense has been paid.
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Gets or sets the actual date the payment was made. Null if not yet paid.
        /// </summary>
        public DateTimeOffset? PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the category of the expense for BI reporting.
        /// </summary>
        public ExpenseCategory Category { get; set; } = ExpenseCategory.Operational;
        #endregion Properties
    }
    #endregion Expense Entity
}