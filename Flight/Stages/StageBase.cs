using Flight.Auditing;
using Flight.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public abstract class StageBase : IStage
    {
        private bool initialized = false;

        protected StageBase()
        {
        }

        protected ILogger Logger { get; private set; } = NullLogger.Instance;

        public virtual void Initialize(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger(GetType());
            initialized = true;
        }

        public async Task MigrateAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (!initialized)
                throw new FlightException("Stage not initialized");

            await ExecuteAsync(connection, batchManager, auditLog, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task ExecuteAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);
    }
}