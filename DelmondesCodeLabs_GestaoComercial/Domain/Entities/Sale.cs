using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using System.Net.ServerSentEvents;

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Sale Entity
    /// <summary>
    /// Represents a completed or pending transaction at the point of sale.
    /// </summary>
    public class Sale : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique, human-readable receipt number for the transaction.
        /// </summary>
        public string ReceiptNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the custom identifier for the tab (e.g., Customer Name, Table Number, or Nickname).
        /// </summary>
        public string CustomerIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the sale was initiated or completed.
        /// </summary>
        public DateTimeOffset SaleDate { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets or sets the gross total of the sale before any discounts.
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Gets or sets the total discount applied to the sale (e.g., from promotions or manual override).
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the final net amount to be paid by the customer.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the method used to pay for the transaction.
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Pending;

        /// <summary>
        /// Gets or sets the current status of the transaction.
        /// </summary>
        public SaleStatus Status { get; set; } = SaleStatus.Pending;

        /// <summary>
        /// Gets or sets the collection of individual items purchased in this sale.
        /// </summary>
        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
        #endregion Properties
    }
    #endregion Sale Entity
}
