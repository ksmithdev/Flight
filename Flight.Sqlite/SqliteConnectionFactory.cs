namespace Flight
{
    using Flight.Database;
    using Microsoft.Data.Sqlite;
    using System;
    using System.Data.Common;

    internal class SqliteConnectionFactory : IConnectionFactory
    {
        private readonly Func<SqliteConnection> connectionFactory;

        public SqliteConnectionFactory(string connectionString)
        {
            connectionFactory = () => new SqliteConnection(connectionString);
        }

        public SqliteConnectionFactory(string dataSource, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate)
        {
            var builder = new SqliteConnectionStringBuilder()
            {
                DataSource = dataSource,
                Mode = sqliteOpenMode
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new SqliteConnection(connectionString);
        }

        public DbConnection Create() => connectionFactory();
    }
}