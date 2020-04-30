namespace Flight
{
    using Flight.Database;
    using Flight.Executors;
    using Flight.Logging;
    using Flight.Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class Migration : IMigration
    {
        private readonly IAuditor auditLog;
        private readonly IBatchManager batchManager;
        private readonly IConnectionFactory connectionFactory;
        private readonly IScriptProvider migrationScriptProvider;
        private readonly IScriptProvider preMigrationScriptProvider;
        private readonly IScriptExecutor scriptExecutor;

        internal Migration(IConnectionFactory connectionFactory, IScriptExecutor scriptExecutor, IAuditor auditLog, IBatchManager batchManager, IScriptProvider preMigrationScriptProvider, IScriptProvider migrationScriptProvider)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.scriptExecutor = scriptExecutor ?? throw new ArgumentNullException(nameof(scriptExecutor));
            this.auditLog = auditLog ?? throw new ArgumentNullException(nameof(auditLog));
            this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));
            this.preMigrationScriptProvider = preMigrationScriptProvider ?? throw new ArgumentNullException(nameof(preMigrationScriptProvider));
            this.migrationScriptProvider = migrationScriptProvider ?? throw new ArgumentNullException(nameof(migrationScriptProvider));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            Log.Info("Migration started");

            try
            {
                using var connection = connectionFactory.Create();
                await connection.OpenAsync().ConfigureAwait(false);

                var initializationScripts = preMigrationScriptProvider.GetScripts();

                foreach (var script in initializationScripts)
                {
                    Log.Info($"Executing pre-migration script {script.ScriptName}, Checksum: {script.Checksum}");
                    Log.Debug(script.Text);

                    foreach (var commandText in batchManager.Split(script))
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = commandText;
                        command.CommandType = System.Data.CommandType.Text;

                        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                await auditLog.EnsureCreatedAsync(connection, cancellationToken).ConfigureAwait(false);
                await auditLog.StoreEntriesAsync(connection, null, initializationScripts, cancellationToken).ConfigureAwait(false);

                var auditLogEntries = await auditLog.LoadEntriesAsync(connection, cancellationToken).ConfigureAwait(false);

                var changeSet = CreateChangeSet(auditLogEntries);
                if (changeSet.Count > 0)
                    await scriptExecutor.ExecuteAsync(connection, changeSet, batchManager, auditLog, cancellationToken).ConfigureAwait(false);
            }
            catch (FlightException)
            {
                throw;
            }
            catch (Exception ex)
            {
                const string UnknownError = "An unknown error occured while migrating the database";
                Log.Error(ex, UnknownError);
                throw new FlightException(UnknownError, ex);
            }

            Log.Info("Migration completed");
        }

        private ICollection<IScript> CreateChangeSet(IEnumerable<AuditEntry> auditLogEntries)
        {
            var entriesLookup = auditLogEntries
                .OrderByDescending(e => e.Applied)
                .ToLookup(e => e.ScriptName, StringComparer.OrdinalIgnoreCase);

            var changeSet = new List<IScript>();
            foreach (var script in migrationScriptProvider.GetScripts())
            {
                var entries = entriesLookup[script.ScriptName];

                if (entries?.Any(e => e.Checksum == script.Checksum) == false)
                {
                    changeSet.Add(script);
                }
                else
                {
                    var lastApplied = entries.FirstOrDefault();

                    if (script.Idempotent && script.Checksum != lastApplied.Checksum)
                    {
                        changeSet.Add(script);
                    }
                }
            }

            return changeSet;
        }
    }
}