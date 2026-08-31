#region Usings
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Category Entity
    /// <summary>
    /// Represents a product category within the commercial management system.
    /// </summary>
    public class Category : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the category.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the category.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this category is active and visible in the point of sale.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the collection of products associated with this category.
        /// </summary>
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
        #endregion
    }
    #endregion
}