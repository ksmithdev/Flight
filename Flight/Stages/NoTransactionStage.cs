using Flight.Auditing;
using Flight.Database;
using Flight.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight.Stages
{
    public class NoTransactionStage : ExecutionStageBase
    {
        public NoTransactionStage(IScriptProvider scriptProvider)
            : base(scriptProvider)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        protected override async Task ApplyAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (scripts == null)
                throw new ArgumentNullException(nameof(scripts));

            if (batchManager == null)
                throw new ArgumentNullException(nameof(batchManager));

            if (auditLog == null)
                throw new ArgumentNullException(nameof(auditLog));

            foreach (var script in scripts)
            {
                Logger.LogInformation($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");
                Logger.LogDebug(script.Text);

                foreach (var commandText in batchManager.Split(script))
                {
                    if (string.IsNullOrWhiteSpace(commandText))
                        continue;

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }

                await auditLog.StoreEntryAsync(connection, null, script, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}