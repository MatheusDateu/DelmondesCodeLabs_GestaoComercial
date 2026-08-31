#region Usings
using DelmondesCodeLabs_GestaoComercial.Domain.Common;
#endregion Usigns
namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Stock Movement Entity
    /// <summary>
    /// Represents an immutable record of any change to a product's inventory level.
    /// </summary>
    public class StockMovement : AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the identifier of the affected product.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the affected product.
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Gets or sets the amount moved. Positive for entries, negative for deductions.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the classification of the movement.
        /// </summary>
        public MovementType Type { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the physical movement occurred.
        /// </summary>
        public DateTimeOffset MovementDate { get; set; }

        /// <summary>
        /// Gets or sets a descriptive reason or reference code for the movement.
        /// </summary>
        public string? Reason { get; set; }
        #endregion Properties
    }
    #endregion Stock Movement Entity
}
