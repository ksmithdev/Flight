namespace Flight.Stages
{
    using Flight.Database;
    using Flight.Logging;
    using Flight.Providers;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public class TransactionStage : MigrationStageBase
    {
        public TransactionStage(IScriptProvider scriptProvider)
            : base(scriptProvider)
        {
        }

        protected override async Task ApplyAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));

            if (batchManager == null)
                throw new ArgumentNullException(nameof(batchManager));

            if (auditor == null)
                throw new ArgumentNullException(nameof(auditor));

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var script in scripts)
                    {
                        Log.Info($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");
                        Log.Debug(script.Text);

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

                        await auditor.StoreEntryAsync(connection, transaction, script, cancellationToken).ConfigureAwait(false);
                    }

                    transaction.Commit();
                }
                catch (System.Exception)
                {
                    transaction.Rollback();

                    throw;
                }
            }
        }
    }
}