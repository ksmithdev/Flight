namespace Flight
{
    using System;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Flight.Database;

    /// <summary>
    /// Represents a factory used for creating connections to SQL Server.
    /// </summary>
    internal class SqlConnectionFactory : IConnectionFactory
    {
        private readonly Func<SqlConnection> connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="dataSource">The name or network address of the instance of SQL Server to connect to.</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        public SqlConnectionFactory(string dataSource, string database)
        {
            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            if (database is null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            var builder = new SqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                DataSource = dataSource,
                InitialCatalog = database,
                IntegratedSecurity = true,
            };
            var connectionString = builder.ToString();

            this.connectionFactory = () => new SqlConnection(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="dataSource">The name or network address of the instance of SQL Server to connect to.</param>
        /// <param name="database">The name of the database associated with the connection.</param>
        /// <param name="userId">The user ID to use when connecting to SQL Server.</param>
        /// <param name="password">The password for the SQL Server account.</param>
        public SqlConnectionFactory(string dataSource, string database, string userId, string password)
        {
            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            if (database is null)
            {
                throw new ArgumentNullException(nameof(database));
            }

            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var builder = new SqlConnectionStringBuilder()
            {
                ApplicationName = "Flight",
                DataSource = dataSource,
                InitialCatalog = database,
                IntegratedSecurity = false,
                UserID = userId,
                Password = password,
            };
            var connectionString = builder.ToString();

            this.connectionFactory = () => new SqlConnection(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection used to open the SQL Server database.</param>
        public SqlConnectionFactory(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.connectionFactory = () => new SqlConnection(connectionString);
        }

        /// <inheritdoc/>
        public DbConnection Create() => this.connectionFactory();
    }
}