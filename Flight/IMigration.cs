namespace Flight
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a migration plan.
    /// </summary>
    public interface IMigration
    {
        /// <summary>
        /// Apply the migration plan to the target database.
        /// </summary>
        /// <param name="cancellationToken">The token used to notify that operations should be canceled.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}