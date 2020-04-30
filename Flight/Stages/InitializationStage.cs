namespace Flight.Stages
{
    using Flight.Database;
    using Flight.Logging;
    using Flight.Providers;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public class InitializationStage : StageBase
    {
        public InitializationStage(IScriptProvider scriptProvider)
        {
            ScriptProvider = scriptProvider ?? throw new ArgumentNullException(nameof(scriptProvider));
        }

        private IScriptProvider ScriptProvider { get; }

        public override async Task MigrateAsync(DbConnection connection, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (batchManager == null)
                throw new ArgumentNullException(nameof(batchManager));

            if (auditor == null)
                throw new ArgumentNullException(nameof(auditor));

            var scripts = ScriptProvider.GetScripts();

            foreach (var script in scripts)
            {
                Log.Info($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");

                foreach (var commandText in batchManager.Split(script))
                {
                    Log.Debug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            await auditor.EnsureCreatedAsync(connection, cancellationToken).ConfigureAwait(false);
            await auditor.StoreEntriesAsync(connection, null, scripts, cancellationToken).ConfigureAwait(false);
        }
    }
}