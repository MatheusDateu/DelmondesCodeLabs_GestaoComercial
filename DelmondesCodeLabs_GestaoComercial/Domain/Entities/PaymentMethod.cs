namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Enums
    /// <summary>
    /// Defines the accepted payment methods at the point of sale.
    /// </summary>
    public enum PaymentMethod
    {
        Cash = 1,
        Pix = 2,
        CreditCard = 3,
        DebitCard = 4,
        /// <summary>
        /// Represents an open tab or a pending payment that has not yet been completed.
        /// </summary>
        Pending = 5
    }
    #endregion Enums
}