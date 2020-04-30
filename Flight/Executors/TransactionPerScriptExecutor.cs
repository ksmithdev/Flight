namespace Flight.Executors
{
    using Flight.Database;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TransactionPerScriptExecutor : IScriptExecutor
    {
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditLog, CancellationToken cancellationToken)
        {
            foreach (var script in scripts)
            {
                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var commandText in batchManager.Split(script))
                    {
                        if (string.IsNullOrWhiteSpace(commandText))
                            continue;

                        using var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandText = commandText;
                        command.CommandType = System.Data.CommandType.Text;

                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

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
        }
    }
}