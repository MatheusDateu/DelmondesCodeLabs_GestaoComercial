#region Usings
using System;
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Product Composition Entity
    /// <summary>
    /// Represents a bill of materials (BOM), linking a parent service or kit to its consumable child components.
    /// </summary>
    public class ProductComposition : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier of the main service or kit being sold.
        /// </summary>
        public Guid ParentProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the main service or kit.
        /// </summary>
        public Product? ParentProduct { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the component that is consumed.
        /// </summary>
        public Guid ChildProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the consumed component.
        /// </summary>
        public Product? ChildProduct { get; set; }

        /// <summary>
        /// Gets or sets the amount of the child component to deduct when the parent is sold.
        /// </summary>
        public decimal Quantity { get; set; }
        #endregion
    }
    #endregion
}