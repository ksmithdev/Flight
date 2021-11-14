namespace Flight.Database
{
    using System.Data.Common;

    /// <summary>
    /// Represents a factory used for creating connections to the database.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Return a connection to the database.
        /// </summary>
        /// <returns>A connection to the database.</returns>
        DbConnection Create();
    }
}