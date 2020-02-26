using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Database
{
    public interface IAuditLog
    {
        Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken = default);

        Task StoreEntriesAsync(DbConnection connection, DbTransaction? transaction, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);

        Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default);

        Task<IEnumerable<AuditLogEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken);
    }
}