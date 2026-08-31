#region Usings
using System;
using System.Collections.Generic;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Stock Entry Entity
    /// <summary>
    /// Represents a formal receipt or invoice of goods from a supplier.
    /// </summary>
    public class StockEntry : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier of the supplier providing the goods.
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the supplier.
        /// </summary>
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Gets or sets the invoice or receipt reference number.
        /// </summary>
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the date the goods were formally received.
        /// </summary>
        public DateTimeOffset EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the total financial cost of the entry.
        /// </summary>
        public decimal TotalCost { get; set; }

        /// <summary>
        /// Gets or sets the collection of line items contained within this stock entry.
        /// </summary>
        public ICollection<StockEntryItem> Items { get; set; } = new HashSet<StockEntryItem>();
        #endregion
    }
    #endregion
}