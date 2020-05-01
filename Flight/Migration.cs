﻿namespace Flight
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
        private readonly IScriptProvider initializationScriptProvider;
        private readonly IScriptProvider migrationScriptProvider;
        private readonly IScriptExecutor scriptExecutor;

        internal Migration(IConnectionFactory connectionFactory, IScriptExecutor scriptExecutor, IAuditor auditLog, IBatchManager batchManager, IScriptProvider initializationScriptProvider, IScriptProvider migrationScriptProvider)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.scriptExecutor = scriptExecutor ?? throw new ArgumentNullException(nameof(scriptExecutor));
            this.auditLog = auditLog ?? throw new ArgumentNullException(nameof(auditLog));
            this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));
            this.initializationScriptProvider = initializationScriptProvider ?? throw new ArgumentNullException(nameof(initializationScriptProvider));
            this.migrationScriptProvider = migrationScriptProvider ?? throw new ArgumentNullException(nameof(migrationScriptProvider));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            Log.Info("Migration started");

            try
            {
                using var connection = connectionFactory.Create();
                await connection.OpenAsync().ConfigureAwait(false);

                var initializationScripts = initializationScriptProvider.GetScripts();

                Log.Info($"{initializationScripts.Count()} initialization script(s) loaded.");
                foreach (var script in initializationScripts)
                {
                    Log.Info($"Executing initialization script {script.ScriptName}, Checksum: {script.Checksum}");
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
                var unknown = FlightExceptionFactory.Unknown(ex);
                Log.Error(unknown, unknown.Message);
                throw unknown;
            }

            Log.Info("Migration completed");
        }

        private ICollection<IScript> CreateChangeSet(IEnumerable<AuditEntry> auditLogEntries)
        {
            var entriesLookup = auditLogEntries
                .OrderByDescending(e => e.Applied)
                .ToLookup(e => e.ScriptName, StringComparer.OrdinalIgnoreCase);

            var changeSet = new List<IScript>();
            var scripts = migrationScriptProvider.GetScripts();

            Log.Info($"{scripts.Count()} migration script(s) loaded.");
            Log.Info("Generating change set...");
            foreach (var script in scripts)
            {
                var entries = entriesLookup[script.ScriptName];

                if (entries?.Any() == false)
                {
                    Log.Debug($"Adding {script.ScriptName} to change set: not applied before");
                    changeSet.Add(script);
                }
                else
                {
                    var lastApplied = entries.FirstOrDefault();

                    Log.Trace($"{script.ScriptName} last applied checksum: {lastApplied.Checksum}");

                    if (script.Checksum != lastApplied.Checksum)
                    {
                        if (script.Idempotent)
                        {
                            Log.Debug($"Adding {script.ScriptName} to change set: idempotent script changed");
                            changeSet.Add(script);
                        }
                        else
                        {
                            Log.Warn($"Script {script.ScriptName} has changed but is not marked as idempotent");
                        }
                    }
                }
            }

            Log.Info($"Change set created with {changeSet.Count} scripts.");
            return changeSet;
        }
    }
}