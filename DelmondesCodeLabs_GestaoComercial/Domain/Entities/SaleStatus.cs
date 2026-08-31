namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Enums
    /// <summary>
    /// Defines the lifecycle status of a sale transaction.
    /// </summary>
    public enum SaleStatus
    {
        /// <summary>
        /// The sale is currently active in the POS cart.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// The sale has been successfully paid and finalised.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// The sale was voided or cancelled by the operator.
        /// </summary>
        Cancelled = 3
    }
    #endregion Enums
}
