﻿using Flight.Database;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Executors
{
    internal class OneTransactionExecutor : IScriptExecutor
    {
        public async Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken)
        {
            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var script in scripts)
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
                }

                await auditLog.StoreEntriesAsync(connection, transaction, scripts, cancellationToken).ConfigureAwait(false);

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