using Flight.Auditing;
using Flight.Database;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public interface IStage
    {
        void Initialize(ILoggerFactory loggerFactory);

        Task MigrateAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);
    }
}