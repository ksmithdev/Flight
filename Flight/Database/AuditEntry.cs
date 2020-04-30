namespace Flight.Database
{
    using System;

    public class AuditEntry
    {
        public DateTimeOffset Applied { get; set; }

        public string AppliedBy { get; set; } = string.Empty;

        public string Checksum { get; set; } = string.Empty;

        public bool Idempotent { get; set; }

        public string ScriptName { get; set; } = string.Empty;
    }
}