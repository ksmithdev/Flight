namespace Flight.Stages
{
    using Flight.Database;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IStage
    {
        Task MigrateAsync(DbConnection connection, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken = default);
    }
}