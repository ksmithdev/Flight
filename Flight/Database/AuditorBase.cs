namespace Flight.Database;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents the abstract auditor base class.
/// </summary>
public abstract class AuditorBase : IAuditor
{
    /// <inheritdoc/>
    public abstract Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public abstract Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public virtual async Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default)
    {
        if (scripts == null)
        {
            throw new ArgumentNullException(nameof(scripts));
        }

        foreach (var script in scripts)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await StoreEntryAsync(connection, transaction, script, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public abstract Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);
}