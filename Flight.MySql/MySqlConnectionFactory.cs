namespace Flight
{
    using Flight.Database;
    using MySql.Data.MySqlClient;
    using System;
    using System.Data.Common;

    internal class MySqlConnectionFactory : IConnectionFactory
    {
        private readonly Func<MySqlConnection> connectionFactory;

        public MySqlConnectionFactory(string connectionString)
        {
            connectionFactory = () => new MySqlConnection(connectionString);
        }

        public MySqlConnectionFactory(string server, string database)
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = server,
                Database = database,
                IntegratedSecurity = true,
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new MySqlConnection(connectionString);
        }

        public MySqlConnectionFactory(string server, string database, string userId, string password)
        {
            var builder = new MySqlConnectionStringBuilder()
            {
                Server = server,
                Database = database,
                UserID = userId,
                Password = password,
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new MySqlConnection(connectionString);
        }

        public DbConnection Create() => connectionFactory();
    }
}