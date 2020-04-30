namespace Flight
{
    using Flight.Database;
    using Flight.Executors;
    using Flight.Logging;
    using Flight.Providers;
    using Microsoft.Extensions.Logging;
    using System;

    public class MigrationBuilder
    {
        private readonly CompositeScriptProvider migrationScriptProvider;
        private readonly CompositeScriptProvider initializationScriptProvider;
        private IAuditor? auditLog;
        private IBatchManager? batchManager;
        private IConnectionFactory? connectionFactory;
        private IScriptExecutor? scriptExecutor;

        public MigrationBuilder()
        {
            initializationScriptProvider = new CompositeScriptProvider();
            migrationScriptProvider = new CompositeScriptProvider();
        }

        public MigrationBuilder AddMigrationScripts(IScriptProvider scriptProvider)
        {
            migrationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        public MigrationBuilder AddInitializationScripts(IScriptProvider scriptProvider)
        {
            initializationScriptProvider.AddScriptProvider(scriptProvider);

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
            if (scriptExecutor == null)
                throw new InvalidOperationException("cannot build migration without setting script executor");

            Log.SetLogger(loggerFactory.CreateLogger(typeof(Migration)));

            return new Migration(
                connectionFactory,
                scriptExecutor,
                auditLog,
                batchManager,
                initializationScriptProvider,
                migrationScriptProvider);
        }

        public void SetAuditLog(IAuditor auditLog) => this.auditLog = auditLog ?? throw new ArgumentNullException(nameof(auditLog));

        public void SetBatchManager(IBatchManager batchManager) => this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));

        public void SetConnectionFactory(IConnectionFactory connectionFactory) => this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        public void SetScriptExecutor(IScriptExecutor scriptExecutor) => this.scriptExecutor = scriptExecutor ?? throw new ArgumentNullException(nameof(scriptExecutor));

        public MigrationBuilder UseNoTransaction()
        {
            SetScriptExecutor(new NoTransactionExecutor());

            return this;
        }

        public MigrationBuilder UseOneTransaction()
        {
            SetScriptExecutor(new TransactionExecutor());

            return this;
        }

        public MigrationBuilder UseTransactionPerScript()
        {
            SetScriptExecutor(new TransactionPerScriptExecutor());

            return this;
        }
    }
}