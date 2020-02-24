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
        protected ExecutionStageBase(IScriptProvider scriptProvider)
        {
            ScriptProvider = scriptProvider;
        }

        protected IScriptProvider ScriptProvider { get; }

        public override void Initialize(ILoggerFactory loggerFactory)
        {
            ScriptProvider.Initialize(loggerFactory);

            base.Initialize(loggerFactory);
        }

        protected abstract Task ApplyAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default);

        protected override async Task ExecuteAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            var scripts = ScriptProvider.GetScripts();
            var changeSet = await auditLog.CreateChangeSetAsync(connection, scripts, cancellationToken).ConfigureAwait(false);

            Logger.LogInformation($"Change set contains {changeSet.Count()} script(s)");

            if (changeSet.Any())
                await ApplyAsync(connection, changeSet, batchManager, auditLog, cancellationToken).ConfigureAwait(false);
        }
    }
}