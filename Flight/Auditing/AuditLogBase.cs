using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Auditing
{
    public abstract class AuditLogBase : IAuditLog
    {
        private ILookup<string, AuditEntry>? auditLog;

        protected AuditLogBase()
        {
        }

        public abstract Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        public abstract Task LogAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);

        public virtual async Task LogAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default)
        {
            foreach (var script in scripts)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await LogAsync(connection, transaction, script, cancellationToken);
            }
        }

        public async Task<string?> ReadLastAppliedChecksumAsync(DbConnection connection, IScript script, CancellationToken cancellationToken = default)
        {
            auditLog ??= await LoadAuditEntriesAsync(connection, cancellationToken);

            var entries = auditLog[script.ScriptName];
            var entry = entries?.FirstOrDefault();

            return entry?.Checksum;
        }

        protected abstract Task<ILookup<string, AuditEntry>> LoadAuditEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

        protected class AuditEntry
        {
            public DateTimeOffset Applied { get; set; }

            public string AppliedBy { get; set; } = string.Empty;

            public string Checksum { get; set; } = string.Empty;

            public bool Idempotent { get; set; }

            public string ScriptName { get; set; } = string.Empty;
        }
    }
}