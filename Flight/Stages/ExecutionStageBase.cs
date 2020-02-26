using Flight.Auditing;
using Flight.Database;
using Flight.Providers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public abstract class ExecutionStageBase : StageBase
    {
        private readonly IScriptProvider scriptProvider;

        protected ExecutionStageBase(IScriptProvider scriptProvider)
        {
            this.scriptProvider = scriptProvider;
        }

        public override void Initialize(ILoggerFactory loggerFactory)
        {
            scriptProvider.Initialize(loggerFactory);

            base.Initialize(loggerFactory);
        }

        protected abstract Task ApplyAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);

        protected override async Task ExecuteAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (auditLog == null)
                throw new System.ArgumentNullException(nameof(auditLog));

            var scripts = scriptProvider.GetScripts();
            var changeSet = await auditLog.CreateChangeSetAsync(connection, scripts, cancellationToken).ConfigureAwait(false);

            Logger.LogInformation($"Change set contains {changeSet.Count()} script(s)");

            if (changeSet.Any())
                await ApplyAsync(connection, changeSet, batchManager, auditLog, cancellationToken).ConfigureAwait(false);
        }
    }
}