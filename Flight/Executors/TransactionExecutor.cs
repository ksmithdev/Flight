namespace Flight.Executors
{
    using Flight.Database;
    using Flight.Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TransactionExecutor : IScriptExecutor
    {
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditLog, CancellationToken cancellationToken)
        {
            Log.Trace($"Begin {nameof(TransactionExecutor)}.{nameof(ExecuteAsync)}");

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var script in scripts)
                {
                    Log.Info($"Executing migration script {script.ScriptName}, Checksum: {script.Checksum}");

                    foreach (var commandText in batchManager.Split(script))
                    {
                        if (string.IsNullOrWhiteSpace(commandText))
                            continue;

                        Log.Debug(commandText);

                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = commandText;
                        command.CommandType = System.Data.CommandType.Text;

                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }

                await auditLog.StoreEntriesAsync(connection, transaction, scripts, cancellationToken).ConfigureAwait(false);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();

                throw;
            }

            Log.Trace($"End {nameof(TransactionExecutor)}.{nameof(ExecuteAsync)}");
        }
    }
}