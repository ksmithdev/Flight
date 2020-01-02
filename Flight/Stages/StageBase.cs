using Flight.Auditing;
using Flight.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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

        public async Task MigrateAsync(IConnectionFactory connectionFactory, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (!initialized)
                throw new FlightException("stage not initialized");

            await ExecuteAsync(connectionFactory, batchManager, auditLog, cancellationToken);
        }

        protected abstract Task ExecuteAsync(IConnectionFactory connectionManager, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);
    }
}