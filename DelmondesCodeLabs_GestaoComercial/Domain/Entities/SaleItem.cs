using DelmondesCodeLabs_GestaoComercial.Domain.Common;

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Sale Item Entity
    /// <summary>
    /// Represents an individual product line within a sale transaction.
    /// </summary>
    public class SaleItem : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the foreign key for the associated sale.
        /// </summary>
        public Guid SaleId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the associated sale.
        /// </summary>
        public Sale? Sale { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the purchased product.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the purchased product.
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product being purchased.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price of a single unit of the product at the time of sale.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets any specific discount applied directly to this line item.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the calculated total for this line item ((Quantity * UnitPrice) - DiscountAmount).
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Gets or sets the current status of this specific item (e.g., Active, Canceled, Damaged).
        /// </summary>
        public SaleItemStatus Status { get; set; } = SaleItemStatus.Active;

        #endregion Properties
    }
    #endregion Sale Item Entity
}
