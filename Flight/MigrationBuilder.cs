namespace Flight
{
    using Flight.Database;
    using Flight.Executors;
    using Flight.Logging;
    using Flight.Providers;
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Defines a migration using the builder pattern.
    /// </summary>
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

        /// <summary>
        /// Add initialization scripts. Initialization scripts are executed prior to ensuring audit table exists and are best used for development and resetting databases to a known state.
        /// </summary>
        /// <remarks>Initialization scripts are not executed in a transaction.</remarks>
        /// <param name="scriptProvider"></param>
        /// <returns></returns>
        public MigrationBuilder AddInitializationScripts(IScriptProvider scriptProvider)
        {
            initializationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        /// <summary>
        /// Add scripts to the migration plan. Batches of migration scripts are executed in order they are added to the plan.
        /// </summary>
        /// <param name="scriptProvider"></param>
        /// <returns></returns>
        public MigrationBuilder AddMigrationScripts(IScriptProvider scriptProvider)
        {
            migrationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        /// <summary>
        /// Build the migration plan.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <exception cref="InvalidOperationException">Thrown when a required resource was not specified.</exception>
        /// <returns>A <see cref="Migration">Migration Plan</see></returns>
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

        /// <summary>
        /// Set the auditor to used for tracking applied scripts. This is usually set by an extension method supplied by the database specific package.
        /// </summary>
        /// <param name="auditor"></param>
        public void SetAuditor(IAuditor auditor) => this.auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));

        /// <summary>
        /// Set the script file batch manager. This is usually set by an extension method supplied by the database specific package.
        /// </summary>
        /// <param name="batchManager"></param>
        public void SetBatchManager(IBatchManager batchManager) => this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));

        /// <summary>
        /// Set the connection string factory. This is usually set by an extension method supplied by the database specific package.
        /// </summary>
        /// <param name="connectionFactory"></param>
        public void SetConnectionFactory(IConnectionFactory connectionFactory) => this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

        /// <summary>
        /// Set the migration plan to execute the migration scripts without any transaction. If an exception occurs then no rollback will occur.
        /// </summary>
        /// <returns></returns>
        public MigrationBuilder UseNoTransaction()
        {
            scriptExecutor = new NoTransactionExecutor();

            return this;
        }

        /// <summary>
        /// Set the migration plan to execute the migration scripts inside a transaction. If an exception occurs then all scripts are rolled back. This is the default executor.
        /// </summary>
        /// <remarks>This is the default executor.</remarks>
        /// <returns></returns>
        public MigrationBuilder UseTransaction()
        {
            scriptExecutor = new TransactionExecutor();

            return this;
        }

        /// <summary>
        /// Set the migration plan to execute the migration scripts inside individual transactions. If an exception occurs then only the script being executed will be rolled back.
        /// </summary>
        /// <returns></returns>
        public MigrationBuilder UseTransactionPerScript()
        {
            scriptExecutor = new TransactionPerScriptExecutor();

            return this;
        }
    }
}