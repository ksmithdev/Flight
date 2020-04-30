namespace Flight.Executors
{
    using Flight.Database;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IScriptExecutor
    {
        Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditLog, CancellationToken cancellationToken);
    }
}