namespace Flight.Database;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents an auditor used to track what scripts have been applied against a database.
/// </summary>
public interface IAuditor
{
    /// <summary>
    /// Ensures that the audit table exists in the supplied connection and creates it if not.
    /// </summary>
    /// <param name="connection">The database connection to target.</param>
    /// <param name="cancellationToken">The token used to notify that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return the collection of audit entries from the audit table.
    /// </summary>
    /// <param name="connection">The database connection to target.</param>
    /// <param name="cancellationToken">The token used to notify that operations should be canceled.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the collection of <see cref="AuditEntry"/> instances.
    /// </returns>
    Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

    /// <summary>
    /// Store the supplied executed scripts into the audit table.
    /// </summary>
    /// <param name="connection">The database connection to target.</param>
    /// <param name="transaction">The optional transaction used when executing scripts.</param>
    /// <param name="scripts">The collection of scripts to execute during the migration.</param>
    /// <param name="cancellationToken">The token used to notify that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Store the supplied executed script into the audit table.
    /// </summary>
    /// <param name="connection">The database connection to target.</param>
    /// <param name="transaction">The optional transaction used when executing scripts.</param>
    /// <param name="script">The script to execute during the migration.</param>
    /// <param name="cancellationToken">The token used to notify that operations should be canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);
}