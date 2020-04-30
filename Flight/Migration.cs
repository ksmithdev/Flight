namespace Flight
{
    using Flight.Database;
    using Flight.Logging;
    using Flight.Stages;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class Migration : IMigration
    {
        private readonly IAuditor auditor;
        private readonly IBatchManager batchManager;
        private readonly IConnectionFactory connectionFactory;
        private readonly IEnumerable<IStage> migrationStages;

        internal Migration(IConnectionFactory connectionFactory, IAuditor auditor, IBatchManager batchManager, IEnumerable<IStage> migrationStages)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));
            this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));
            this.migrationStages = migrationStages ?? throw new ArgumentNullException(nameof(migrationStages));
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            Log.Info("Migration started");

            try
            {
                using var connection = connectionFactory.Create();
                await connection.OpenAsync().ConfigureAwait(false);

                foreach (var stage in migrationStages)
                {
                    await stage.MigrateAsync(connection, batchManager, auditor, cancellationToken).ConfigureAwait(false);
                }
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
    }
}