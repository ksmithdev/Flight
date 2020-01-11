using System.Threading;
using System.Threading.Tasks;

namespace Flight
{
    public interface IMigration
    {
        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}