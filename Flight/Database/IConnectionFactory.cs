namespace Flight.Database
{
    using System.Data.Common;

    /// <summary>
    /// Defines a factory used for creating connections to the database.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Return a connection to the database.
        /// </summary>
        /// <returns></returns>
        DbConnection Create();
    }
}