namespace Flight
{
    using System;
    using System.Data.Common;
    using Flight.Database;
    using Microsoft.Data.Sqlite;

    /// <summary>
    /// Represents a factory used for creating connections to SQLite.
    /// </summary>
    internal class SqliteConnectionFactory : IConnectionFactory
    {
        private readonly Func<SqliteConnection> connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The string used to open the connection.</param>
        public SqliteConnectionFactory(string connectionString)
        {
            this.connectionFactory = () => new SqliteConnection(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteConnectionFactory"/> class.
        /// </summary>
        /// <param name="dataSource">The database file.</param>
        /// <param name="sqliteOpenMode">The connection mode.</param>
        public SqliteConnectionFactory(string dataSource, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate)
        {
            var builder = new SqliteConnectionStringBuilder()
            {
                DataSource = dataSource,
                Mode = sqliteOpenMode,
            };
            var connectionString = builder.ToString();

            this.connectionFactory = () => new SqliteConnection(connectionString);
        }

        /// <inheritdoc/>
        public DbConnection Create() => this.connectionFactory();
    }
}