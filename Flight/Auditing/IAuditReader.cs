using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Auditing
{
    public interface IAuditReader
    {
        Task<IEnumerable<IScript>> CreateChangeSetAsync(DbConnection connection, IEnumerable<IScript> scripts, CancellationToken cancellationToken = default);
    }
}