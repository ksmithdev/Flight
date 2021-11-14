namespace Flight.Executors
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Flight.Database;
    using Flight.Logging;

    /// <summary>
    /// Represents a script executor that runs each script in its own transaction.
    /// </summary>
    internal class TransactionPerScriptExecutor : IScriptExecutor
    {
        /// <inheritdoc/>
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditLog, CancellationToken cancellationToken)
        {
            Log.Trace($"Begin {nameof(TransactionPerScriptExecutor)}.{nameof(this.ExecuteAsync)}");

            foreach (var script in scripts)
            {
                Log.Info($"Executing migration script {script.ScriptName}, Checksum: {script.Checksum}");

                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var commandText in batchManager.Split(script))
                    {
                        if (string.IsNullOrWhiteSpace(commandText))
                        {
                            continue;
                        }

                        Log.Debug(commandText);

                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = commandText;
                        command.CommandType = System.Data.CommandType.Text;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }

                    await auditLog.StoreEntryAsync(connection, transaction, script, cancellationToken).ConfigureAwait(false);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    throw;
                }
            }

            Log.Trace($"End {nameof(TransactionPerScriptExecutor)}.{nameof(this.ExecuteAsync)}");
        }
    }
}