namespace DelmondesCodeLabs_GestaoComercial.Domain.Common
{
    #region Audit Log
    /// <summary>
    /// Represents an immutable audit log entry detailing data modifications and CQRS commands.
    /// </summary>
    public class AuditLog
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the audit log.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who performed the action.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of action performed.
        /// </summary>
        public AuditType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the table or entity that was modified.
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the CQRS command that triggered the modification.
        /// </summary>
        public string CommandName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JSON representation of the entity's state before the modification.
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// Gets or sets the JSON representation of the entity's state after the modification.
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the error message or reason for failure, if any.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the audit log was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
        #endregion
    }
    #endregion
}
