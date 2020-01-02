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
    public class Migration
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

                foreach (var stage in stages)
                {
                    logger.LogDebug($"Migrating stage {stage.GetType()}");

                    await stage.MigrateAsync(connectionFactory, batchManager, auditLog, cancellationToken);
                }
            }
            catch (FlightException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration failed due to an unknown exception");

                throw new FlightException("An unknown error occured while migrating the database", ex);
            }

            logger.LogTrace($"Migration complete");
        }
    }
}