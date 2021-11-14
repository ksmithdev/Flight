namespace Flight.Executors
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Flight.Database;
    using Flight.Logging;

    /// <summary>
    /// Represents a script executor that uses no transactions.
    /// </summary>
    internal class NoTransactionExecutor : IScriptExecutor
    {
        /// <inheritdoc/>
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken)
        {
            Log.Trace($"Begin {nameof(NoTransactionExecutor)}.{nameof(this.ExecuteAsync)}");

            foreach (var script in scripts)
            {
                Log.Info($"Executing migration script {script.ScriptName}, Checksum: {script.Checksum}");

                foreach (var commandText in batchManager.Split(script))
                {
                    if (string.IsNullOrWhiteSpace(commandText))
                    {
                        continue;
                    }

                    Log.Debug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }

                await auditor.StoreEntryAsync(connection, null, script, cancellationToken).ConfigureAwait(false);
            }

            Log.Trace($"End {nameof(NoTransactionExecutor)}.{nameof(this.ExecuteAsync)}");
        }
    }
}