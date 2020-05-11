namespace Flight
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a migration plan.
    /// </summary>
    public interface IMigration
    {
        /// <summary>
        /// Apply the migration plan to the target database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}