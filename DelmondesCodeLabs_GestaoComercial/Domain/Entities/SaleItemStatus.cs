namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Enums
    /// <summary>
    /// Defines the operational status of an individual item within a sale/tab.
    /// </summary>
    public enum SaleItemStatus
    {
        /// <summary>
        /// The item is active and has been deducted from stock.
        /// </summary>
        Active = 1,

        /// <summary>
        /// The item was cancelled by the customer or waiter. Stock is returned via MovementType.Entry.
        /// </summary>
        Canceled = 2,

        /// <summary>
        /// The item was damaged or lost (Avaria). Stock is NOT returned, but logged as MovementType.Loss.
        /// </summary>
        Damaged = 3
    }
    #endregion Enums
}
