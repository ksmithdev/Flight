namespace Flight
{
    using Flight.Database;
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;

    internal class SqlConnectionFactory : IConnectionFactory
    {
        private readonly Func<SqlConnection> connectionFactory;

        public SqlConnectionFactory(string dataSource, string database)
        {
            if (dataSource is null)
                throw new ArgumentNullException(nameof(dataSource));
            if (database is null)
                throw new ArgumentNullException(nameof(database));

            var builder = new SqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                DataSource = dataSource,
                InitialCatalog = database,
                IntegratedSecurity = true,
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new SqlConnection(connectionString);
        }

        public SqlConnectionFactory(string dataSource, string database, string userId, string password)
        {
            if (dataSource is null)
                throw new ArgumentNullException(nameof(dataSource));
            if (database is null)
                throw new ArgumentNullException(nameof(database));
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));
            if (password is null)
                throw new ArgumentNullException(nameof(password));

            var builder = new SqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                DataSource = dataSource,
                InitialCatalog = database,
                IntegratedSecurity = false,
                UserID = userId,
                Password = password
            };
            var connectionString = builder.ToString();

            connectionFactory = () => new SqlConnection(connectionString);
        }

        public SqlConnectionFactory(string connectionString)
        {
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            connectionFactory = () => new SqlConnection(connectionString);
        }

        public DbConnection Create() => connectionFactory();
    }
}