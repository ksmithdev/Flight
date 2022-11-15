namespace Flight;

using System;
using System.Data.Common;
using Flight.Database;
using MySql.Data.MySqlClient;

/// <summary>
/// Represents a factory used for creating connections to MySQL.
/// </summary>
internal class MySqlConnectionFactory : IConnectionFactory
{
    private readonly Func<MySqlConnection> connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionString">The connection properties use to open the MySQL database.</param>
    public MySqlConnectionFactory(string connectionString)
    {
        this.connectionFactory = () => new MySqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="server">The name of the server.</param>
    /// <param name="database">The name of te database for the initial connection.</param>
    public MySqlConnectionFactory(string server, string database)
    {
        var builder = new MySqlConnectionStringBuilder()
        {
            Server = server,
            Database = database,
            IntegratedSecurity = true,
        };
        var connectionString = builder.ToString();

        this.connectionFactory = () => new MySqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlConnectionFactory"/> class.
    /// </summary>
    /// <param name="server">The name of the server.</param>
    /// <param name="database">The name of te database for the initial connection.</param>
    /// <param name="userId">The user ID that should be used to connect with.</param>
    /// <param name="password">The password that should be used to make the connection.</param>
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

        this.connectionFactory = () => new MySqlConnection(connectionString);
    }

    /// <inheritdoc/>
    public DbConnection Create() => this.connectionFactory();
}