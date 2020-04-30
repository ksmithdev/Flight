namespace Flight.Database
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAuditor
    {
        Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);

        Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

        Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);

        Task<IEnumerable<IScript>> CreateChangeSetAsync(DbConnection connection, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);
    }
}