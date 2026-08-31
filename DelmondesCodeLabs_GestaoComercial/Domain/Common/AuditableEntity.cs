#region Usings
using System;
#endregion

namespace DelmondesCodeLabs_GestaoComercial.Domain.Common
{
    #region Auditable Entity Base
    /// <summary>
    /// Base class providing auditable properties for all domain entities.
    /// Tracks creation and modification timestamps alongside user identifiers.
    /// </summary>
    public abstract class AuditableEntity
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the entity.
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is marked as deleted (soft delete).
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Gets or sets the version of the entity for concurrency control.
        /// </summary>
        public Guid Version { get; set; } = Guid.NewGuid();
        #endregion
    }
    #endregion
}