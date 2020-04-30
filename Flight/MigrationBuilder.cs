namespace Flight
{
    using Flight.Database;
    using Flight.Logging;
    using Flight.Providers;
    using Flight.Stages;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;

    public class MigrationBuilder
    {
        private IAuditor? auditLog;
        private IBatchManager? batchManager;
        private IConnectionFactory? connectionFactory;
        private IScriptProvider initializationScriptProvider = NullScriptProvider.Instance;
        private readonly ICollection<IStage> migrationStages;

        public MigrationBuilder()
        {
            migrationStages = new List<IStage>();
        }

        public MigrationBuilder AddMigrationStage(IStage stage)
        {
            migrationStages.Add(stage);
            return this;
        }

        public MigrationBuilder RemoveMigrationStage(IStage stage)
        {
            migrationStages.Remove(stage);
            return this;
        }

        public MigrationBuilder AddInitializationScripts(IScriptProvider scriptProvider)
        {
            this.initializationScriptProvider = scriptProvider;

            return this;
        }

        public IMigration Build(ILoggerFactory loggerFactory)
        {
            if (connectionFactory == null)
                throw new InvalidOperationException("cannot build migration without setting connection factory");
            if (batchManager == null)
                throw new InvalidOperationException("cannot build migration without setting batch manager");
            if (auditLog == null)
                throw new InvalidOperationException("cannot build migration without setting audit log");

            Log.SetLogger(loggerFactory.CreateLogger(typeof(Migration)));

            var migrationStages = new List<IStage>();
            migrationStages.Add(new InitializationStage(initializationScriptProvider));
            foreach (var migrationStage in this.migrationStages)
                migrationStages.Add(migrationStage);

            return new Migration(
                connectionFactory,
                auditLog,
                batchManager,
                migrationStages);
        }

        public void SetAuditLog(IAuditor auditLog) => this.auditLog = auditLog ?? throw new ArgumentNullException(nameof(auditLog));

        public void SetBatchManager(IBatchManager batchManager) => this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));

        public void SetConnectionFactory(IConnectionFactory connectionFactory) => this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
}