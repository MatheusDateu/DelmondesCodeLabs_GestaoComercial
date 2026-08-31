#region Usings
using System;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion Usings

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Promotion Item Entity
    /// <summary>
    /// Represents a link between a promotion and a specific product or an entire category.
    /// </summary>
    public class PromotionItem : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the foreign key for the associated promotion.
        /// </summary>
        public Guid PromotionId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the associated promotion.
        /// </summary>
        public Promotion? Promotion { get; set; }

        /// <summary>
        /// Gets or sets the optional foreign key for a specific product. 
        /// Mutually exclusive with CategoryId in practice.
        /// </summary>
        public Guid? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the specific product.
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Gets or sets the optional foreign key for a specific category.
        /// Mutually exclusive with ProductId in practice.
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the specific category.
        /// </summary>
        public Category? Category { get; set; }
        #endregion Properties
    }
    #endregion Promotion Item Entity
}