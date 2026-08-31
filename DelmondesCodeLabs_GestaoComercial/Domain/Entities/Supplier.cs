#region Usings
using System;
using System.Collections.Generic;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Supplier Entity
    /// <summary>
    /// Represents a vendor or supplier who provides goods for the inventory.
    /// </summary>
    public class Supplier : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the corporate name of the supplier.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the corporate taxpayer registry number (CNPJ).
        /// </summary>
        public string? Cnpj { get; set; }

        /// <summary>
        /// Gets or sets the primary contact email address.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the primary contact telephone number.
        /// </summary>
        public string? ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the collection of stock entries associated with this supplier.
        /// </summary>
        public ICollection<StockEntry> StockEntries { get; set; } = new HashSet<StockEntry>();
        #endregion
    }
    #endregion
}