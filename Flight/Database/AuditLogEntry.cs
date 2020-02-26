using System;

namespace Flight.Database
{
    public class AuditLogEntry
    {
        public DateTimeOffset Applied { get; set; }

        public string AppliedBy { get; set; } = string.Empty;

        public string Checksum { get; set; } = string.Empty;

        public bool Idempotent { get; set; }

        public string ScriptName { get; set; } = string.Empty;
    }
}