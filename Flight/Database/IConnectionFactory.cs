namespace Flight.Database
{
    using System.Data.Common;

    public interface IConnectionFactory
    {
        DbConnection Create();
    }
}