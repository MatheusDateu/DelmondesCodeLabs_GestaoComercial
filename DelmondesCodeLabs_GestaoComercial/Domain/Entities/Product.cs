using DelmondesCodeLabs_GestaoComercial.Domain.Common;

namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Product Entity
    /// <summary>
    /// Represents a sellable item, service, or component in the commercial management system.
    /// Designed to handle precise financial values and concurrency safely.
    /// </summary>
    public class Product : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique stock keeping unit identifier.
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the barcode for point of sale scanning.
        /// </summary>
        public string? Barcode { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the cost price of the product.
        /// </summary>
        public decimal CostPrice { get; set; }

        /// <summary>
        /// Gets or sets the selling price of the product in the point of sale.
        /// </summary>
        public decimal SellingPrice { get; set; }

        /// <summary>
        /// Gets or sets the current cached stock quantity. 
        /// </summary>
        public decimal CurrentStock { get; set; }

        /// <summary>
        /// Gets or sets the minimum stock threshold that triggers a business intelligence alert.
        /// </summary>
        public decimal MinimumStockAlert { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is available for sale.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the type of the product, determining how it interacts with inventory.
        /// </summary>
        public ProductType Type { get; set; } = ProductType.Standard;

        /// <summary>
        /// Gets or sets the foreign key for the associated category.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the associated category.
        /// </summary>
        public Category? Category { get; set; }
        #endregion
    }
    #endregion
}
