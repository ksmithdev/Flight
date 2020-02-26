using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Database
{
    public abstract class AuditLogBase : IAuditLog
    {
        public abstract Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        public abstract Task<IEnumerable<AuditLogEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

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
    }
}