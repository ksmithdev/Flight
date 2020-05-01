namespace Flight.Executors
{
    using Flight.Database;
    using Flight.Logging;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal class NoTransactionExecutor : IScriptExecutor
    {
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditLog, CancellationToken cancellationToken)
        {
            Log.Trace($"Begin {nameof(NoTransactionExecutor)}.{nameof(ExecuteAsync)}");

            foreach (var script in scripts)
            {
                Log.Info($"Executing migration script {script.ScriptName}, Checksum: {script.Checksum}");

                foreach (var commandText in batchManager.Split(script))
                {
                    if (string.IsNullOrWhiteSpace(commandText))
                        continue;

                    Log.Debug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }

                await auditLog.StoreEntryAsync(connection, null, script, cancellationToken).ConfigureAwait(false);
            }

            Log.Trace($"End {nameof(NoTransactionExecutor)}.{nameof(ExecuteAsync)}");
        }
    }
}