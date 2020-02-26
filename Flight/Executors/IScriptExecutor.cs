using Flight.Database;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Executors
{
    public interface IScriptExecutor
    {
        Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken);
    }
}