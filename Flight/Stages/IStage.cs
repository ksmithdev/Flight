using Flight.Auditing;
using Flight.Database;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public interface IStage
    {
        void Initialize(ILoggerFactory loggerFactory);

        Task MigrateAsync(IConnectionFactory connectionFactory, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);
    }
}