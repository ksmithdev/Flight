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

        public async Task<string?> ReadLastAppliedChecksumAsync(DbConnection connection, IScript script, CancellationToken cancellationToken = default)
        {
            auditLog ??= await GenerateAuditLogAsync(connection, cancellationToken).ConfigureAwait(false);

            var entries = auditLog[script.ScriptName];
            var entry = entries?.FirstOrDefault();

            return entry?.Checksum;
        }

        public virtual async Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default)
        {
            foreach (var script in scripts)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await StoreEntryAsync(connection, transaction, script, cancellationToken).ConfigureAwait(false);
            }
        }

        public abstract Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);

        protected abstract Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

        private async Task<ILookup<string, AuditEntry>> GenerateAuditLogAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var entries = await LoadEntriesAsync(connection, cancellationToken).ConfigureAwait(false);

            return entries.OrderByDescending(e => e.Applied).ToLookup(e => e.ScriptName, StringComparer.OrdinalIgnoreCase);
        }

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