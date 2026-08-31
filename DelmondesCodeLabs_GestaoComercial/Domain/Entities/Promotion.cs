#region Usings
using System;
using System.Collections.Generic;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Promotion Entity
    /// <summary>
    /// Represents a discount promotion valid within a specific timeframe.
    /// </summary>
    public class Promotion : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the promotion (e.g., 'Black Friday', 'Happy Hour').
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date and time for the promotion.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date and time for the promotion.
        /// </summary>
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Gets or sets the type of discount (Percentage or Fixed Monetary Value).
        /// </summary>
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        /// <summary>
        /// Gets or sets the value of the discount applied to the items or categories.
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the promotion can be applied at the POS.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the collection of items (products or categories) attached to this promotion.
        /// </summary>
        public ICollection<PromotionItem> Items { get; set; } = new List<PromotionItem>();
        #endregion Properties
    }
    #endregion Promotion Entity
}