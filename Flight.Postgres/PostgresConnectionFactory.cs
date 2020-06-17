namespace Flight
{
    using Flight.Database;
    using Npgsql;
    using System;
    using System.Data.Common;

    internal class PostgresConnectionFactory : IConnectionFactory
    {
        private readonly Func<NpgsqlConnection> connectionFactory;

        public PostgresConnectionFactory(string connectionString)
        {
            connectionFactory = () => new NpgsqlConnection(connectionString);
        }

        public PostgresConnectionFactory(string host, string database)
        {
            var builder = new NpgsqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                Host = host,
                Database = database,
                IntegratedSecurity = true,
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new NpgsqlConnection(connectionString);
        }

        public PostgresConnectionFactory(string host, string database, string username, string password)
        {
            var builder = new NpgsqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                Host = host,
                Database = database,
                Username = username,
                Password = password,
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new NpgsqlConnection(connectionString);
        }

        public DbConnection Create() => connectionFactory();
    }
}