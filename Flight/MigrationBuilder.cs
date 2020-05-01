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
        private readonly CompositeScriptProvider initializationScriptProvider;
        private readonly CompositeScriptProvider migrationScriptProvider;
        private IAuditor? auditor;
        private IBatchManager? batchManager;
        private IConnectionFactory? connectionFactory;
        private IScriptExecutor? scriptExecutor;

        public MigrationBuilder()
        {
            initializationScriptProvider = new CompositeScriptProvider();
            migrationScriptProvider = new CompositeScriptProvider();
        }

        public MigrationBuilder AddInitializationScripts(IScriptProvider scriptProvider)
        {
            initializationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        public MigrationBuilder AddMigrationScripts(IScriptProvider scriptProvider)
        {
            migrationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        public IMigration Build(ILoggerFactory loggerFactory)
        {
            if (connectionFactory == null)
                throw FlightExceptionFactory.InvalidOperation("CannotBuildWithoutConnectionFactory");
            if (batchManager == null)
                throw FlightExceptionFactory.InvalidOperation("CannotBuildWithoutBatchManager");
            if (auditor == null)
                throw FlightExceptionFactory.InvalidOperation("CannotBuildWithoutAuditor");
            if (scriptExecutor == null)
                throw FlightExceptionFactory.InvalidOperation("CannotBuildWithoutScriptExecutor");

            Log.SetLogger(loggerFactory.CreateLogger(typeof(Migration)));

            return new Migration(
                connectionFactory,
                scriptExecutor,
                auditor,
                batchManager,
                initializationScriptProvider,
                migrationScriptProvider);
        }

        public void SetAuditor(IAuditor auditor) => this.auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));

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