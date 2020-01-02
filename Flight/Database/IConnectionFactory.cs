using System.Data.Common;

namespace Flight.Database
{
    public interface IConnectionFactory
    {
        DbConnection Create();
    }
}