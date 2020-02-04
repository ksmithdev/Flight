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
            var changeSet = await CreateChangeSetAsync(connection, auditLog).ConfigureAwait(false);

            Logger.LogInformation($"Change set contains {changeSet.Count} script(s)");

            if (changeSet.Count > 0)
                await ApplyAsync(connection, changeSet, batchManager, auditLog, cancellationToken).ConfigureAwait(false);
        }

        private async Task<List<IScript>> CreateChangeSetAsync(DbConnection connection, IAuditLog auditLog)
        {
            var scripts = ScriptProvider.GetScripts();

            Logger.LogInformation($"Generating change set from {scripts.Count()} scripts...");

            var changeSet = new List<IScript>();
            foreach (var script in scripts)
            {
                Logger.LogDebug($"Comparing {script.ScriptName}, checksum={script.Checksum}, idempotent={script.Idempotent}");
                var checksum = await auditLog.ReadLastAppliedChecksumAsync(connection, script).ConfigureAwait(false);
                if (checksum == null)
                {
                    Logger.LogDebug($"No existing checksum found for {script.ScriptName}. Adding to change set.");
                    changeSet.Add(script);
                }
                else if (script.Checksum != checksum)
                {
                    Logger.LogDebug($"Checksum for {script.ScriptName} does not match last applied: {checksum}");
                    if (script.Idempotent)
                    {
                        Logger.LogDebug($"Script is idempotent, adding to change set.");
                        changeSet.Add(script);
                    }
                    else
                    {
                        Logger.LogWarning($"Non-idempotent script {script.ScriptName} has already been applied but has a different checksum");
                    }
                }
                else
                {
                    Logger.LogDebug($"Checksum for {script.ScriptName} matches last applied. Skipping.");
                }
            }

            return changeSet;
        }
    }
}