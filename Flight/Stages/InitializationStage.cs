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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        protected override async Task ExecuteAsync(IConnectionFactory connectionFactory, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            using var connection = connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            Logger.LogDebug($"Connection established");

            var scripts = ScriptProvider.GetScripts();

            foreach (var script in scripts)
            {
                Logger.LogInformation($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");

                var batches = batchManager.Split(script);

                foreach (var commandText in batches)
                {
                    Logger.LogDebug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            await auditLog.EnsureCreatedAsync(connection, cancellationToken);
            await auditLog.LogAsync(connection, null, scripts, cancellationToken);
        }

        protected async override Task ExecuteAsync(DbConnection connection, IBatchManager batchManager, IAuditLog auditLog, CancellationToken cancellationToken = default)
        {
            var scripts = ScriptProvider.GetScripts();

            foreach (var script in scripts)
            {
                Logger.LogInformation($"Applying {script.ScriptName}, Checksum: {script.Checksum}, Idempotent: {script.Idempotent}");

                var batches = batchManager.Split(script);

                foreach (var commandText in batches)
                {
                    Logger.LogDebug(commandText);

                    using var command = connection.CreateCommand();
                    command.CommandText = commandText;
                    command.CommandType = System.Data.CommandType.Text;

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }

            await auditLog.EnsureCreatedAsync(connection, cancellationToken);
            await auditLog.LogAsync(connection, null, scripts, cancellationToken);
        }
    }
}