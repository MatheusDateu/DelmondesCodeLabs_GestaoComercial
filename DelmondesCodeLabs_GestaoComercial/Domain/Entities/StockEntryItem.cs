#region Usings
using System;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Stock Entry Item Entity
    /// <summary>
    /// Represents an individual line item within a stock entry, detailing product, quantity, and cost.
    /// </summary>
    public class StockEntryItem : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier of the parent stock entry.
        /// </summary>
        public Guid StockEntryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the parent stock entry.
        /// </summary>
        public StockEntry? StockEntry { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the received product.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the received product.
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product received.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the cost per unit of the product at the time of entry.
        /// </summary>
        public decimal UnitCost { get; set; }
        #endregion
    }
    #endregion
}