namespace Flight.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AuditorBase : IAuditor
    {
        public abstract Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken = default);

        public virtual async Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default)
        {
            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));

            foreach (var script in scripts)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await StoreEntryAsync(connection, transaction, script, cancellationToken).ConfigureAwait(false);
            }
        }

        public abstract Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);

        public virtual async Task<IEnumerable<IScript>> CreateChangeSetAsync(DbConnection connection, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default)
        {
            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));

            var auditLogEntries = await LoadEntriesAsync(connection, cancellationToken).ConfigureAwait(false);

            var entriesLookup = auditLogEntries
                .OrderByDescending(e => e.Applied)
                .ToLookup(e => e.ScriptName, StringComparer.OrdinalIgnoreCase);

            var changeSet = new List<IScript>();
            foreach (var script in scripts)
            {
                var entries = entriesLookup[script.ScriptName];

                if (entries?.Any(e => e.Checksum == script.Checksum) == false)
                {
                    changeSet.Add(script);
                }
                else
                {
                    var lastApplied = entries.FirstOrDefault();

                    if (script.Idempotent && script.Checksum != lastApplied.Checksum)
                    {
                        changeSet.Add(script);
                    }
                }
            }

            return changeSet;
        }
    }
}