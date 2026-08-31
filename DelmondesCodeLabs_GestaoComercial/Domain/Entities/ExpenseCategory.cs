namespace DelmondesCodeLabs_GestaoComercial.Domain.Entities
{
    #region Enums
    /// <summary>
    /// Defines the categorisation of operational expenses for financial reporting.
    /// </summary>
    public enum ExpenseCategory
    {
        Operational = 1,
        Rent = 2,
        Utilities = 3,
        Payroll = 4,
        Maintenance = 5,
        Taxes = 6,
        Other = 99
    }
    #endregion Enums
}