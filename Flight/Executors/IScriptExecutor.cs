namespace Flight.Executors
{
    using Flight.Database;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a script executor for applying scripts against a database.
    /// </summary>
    public interface IScriptExecutor
    {
        /// <summary>
        /// Execute the collection of scripts against the supplied database connection.
        /// </summary>
        /// <param name="connection">The database connection to apply the scripts against.</param>
        /// <param name="scripts">The collection of scripts to apply.</param>
        /// <param name="batchManager">The batch manager used to split an individual script into batches.</param>
        /// <param name="auditor">The auditor used to keep track of what scripts have been applied to a database.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecuteAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken);
    }
}