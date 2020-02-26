using Flight.Auditing;
using Flight.Database;
using Flight.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public class InitializationStage : StageBase
    {
        public InitializationStage(IScriptProvider scriptProvider)
        {
            ScriptProvider = scriptProvider ?? throw new ArgumentNullException(nameof(scriptProvider));
        }

        private IScriptProvider ScriptProvider { get; }

        public override void Initialize(ILoggerFactory loggerFactory)
        {
            ScriptProvider.Initialize(loggerFactory);

            base.Initialize(loggerFactory);
        }

        protected override async Task ExecuteAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (batchManager == null)
                throw new ArgumentNullException(nameof(batchManager));

            if (auditLog == null)
                throw new ArgumentNullException(nameof(auditLog));

            var scripts = ScriptProvider.GetScripts();

            foreach (var script in scripts)
            {
                Logger.LogInformation($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");

                foreach (var commandText in batchManager.Split(script))
                {
                    Logger.LogDebug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            await auditLog.EnsureCreatedAsync(connection, cancellationToken).ConfigureAwait(false);
            await auditLog.StoreEntriesAsync(connection, null, scripts, cancellationToken).ConfigureAwait(false);
        }
    }
}