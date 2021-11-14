namespace Flight.Database
{
    using System;

    /// <summary>
    /// Represents the audit entry DTO.
    /// </summary>
    public class AuditEntry
    {
        /// <summary>
        /// Gets or sets the date/time the entry was applied.
        /// </summary>
        public DateTimeOffset Applied { get; set; }

        /// <summary>
        /// Gets or sets the user that applied the entry.
        /// </summary>
        public string AppliedBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the script checksum.
        /// </summary>
        public string Checksum { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the scripts is idempotent.
        /// </summary>
        public bool Idempotent { get; set; }

        /// <summary>
        /// Gets or sets the name of the script.
        /// </summary>
        public string ScriptName { get; set; } = string.Empty;
    }
}