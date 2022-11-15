﻿namespace Flight;

using System;
using System.Data.Common;
using Flight.Database;
using Npgsql;

/// <summary>
/// Represents a factory used for creating connections to PostgreSQL.
/// </summary>
internal class PostgresConnectionFactory : IConnectionFactory
{
    private readonly Func<NpgsqlConnection> connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresConnectionFactory"/> class.
    /// </summary>
    /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
    public PostgresConnectionFactory(string connectionString)
    {
        this.connectionFactory = () => new NpgsqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresConnectionFactory"/> class.
    /// </summary>
    /// <param name="host">The hostname or IP address of the PostgreSQL server to connect to.</param>
    /// <param name="database">The PostgreSQL database to connect to.</param>
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

        this.connectionFactory = () => new NpgsqlConnection(connectionString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresConnectionFactory"/> class.
    /// </summary>
    /// <param name="host">The hostname or IP address of the PostgreSQL server to connect to.</param>
    /// <param name="database">The PostgreSQL database to connect to.</param>
    /// <param name="username">The username to connect with.</param>
    /// <param name="password">The password to connect with.</param>
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

        this.connectionFactory = () => new NpgsqlConnection(connectionString);
    }

    /// <inheritdoc/>
    public DbConnection Create() => this.connectionFactory();
}