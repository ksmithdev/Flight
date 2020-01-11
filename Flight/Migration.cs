using Flight.Auditing;
using Flight.Database;
using Flight.Stages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flight
{
    public class Migration : IMigration
    {
        private readonly IAuditLog auditLog;
        private readonly IBatchManager batchManager;
        private readonly IConnectionFactory connectionFactory;
        private readonly ILogger logger;
        private readonly IEnumerable<IStage> stages;

        public Migration(IConnectionFactory connectionFactory, IBatchManager batchManager, IAuditLog auditLog, IEnumerable<IStage> stages, ILoggerFactory loggerFactory)
        {
            this.connectionFactory = connectionFactory;
            this.batchManager = batchManager;
            this.auditLog = auditLog;
            this.stages = stages;

            logger = loggerFactory.CreateLogger(GetType());
        }

        public async Task MigrateAsync(CancellationToken cancellationToken = default)
        {
            logger.LogTrace($"Migration started");

            try
            {
                logger.LogDebug($"{stages.Count()} stages loaded");

                using var connection = connectionFactory.Create();
                await connection.OpenAsync();

                logger.LogDebug($"Successfully established connection to {connection.ConnectionString}");

                foreach (var stage in stages)
                {
                    logger.LogDebug($"Migrating stage {stage.GetType()}");

                    await stage.MigrateAsync(connection, batchManager, auditLog, cancellationToken);
                }
            }
            catch (FlightException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FlightException("An unknown error occured while migrating the database", ex);
            }

            logger.LogTrace($"Migration complete");
        }
    }
}