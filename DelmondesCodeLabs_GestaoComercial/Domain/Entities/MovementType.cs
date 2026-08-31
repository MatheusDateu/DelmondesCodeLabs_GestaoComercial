namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Enums
    /// <summary>
    /// Defines the categorisation of a stock movement for traceability.
    /// </summary>
    public enum MovementType
    {
        Entry = 1,
        Sale = 2,
        Loss = 3,
        InternalConsumption = 4
    }
    #endregion
}
