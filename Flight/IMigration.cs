namespace Flight
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMigration
    {
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}