namespace Flight.Database
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines an auditor used to track what scripts have been applied against a database.
    /// </summary>
    public interface IAuditor
    {
        /// <summary>
        /// Ensures that the audit table exists in the supplied connection and creates it if not.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        /// <summary>
        /// Return the collection of audit entries from the audit table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

        /// <summary>
        /// Store the supplied executed scripts into the audit table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="scripts"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

        /// <summary>
        /// Store the supplied executed script into the audit table.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="script"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);
    }
}