using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Auditing
{
    public interface IAuditLog
    {
        Task<IEnumerable<IScript>> CreateChangeSetAsync(DbConnection connection, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

        Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

        Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);
    }
}