namespace Flight;

using System;

/// <summary>
/// The collection of MySQL extension methods for the <see cref="MigrationBuilder"/>.
/// </summary>
public static class MySqlMigrationBuilderExtensions
{
    /// <summary>
    /// Apply the MySQL connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="connectionString">The connection properties use to open the MySQL database.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string connectionString, string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new MySqlConnectionFactory(connectionString);
        var auditor = new MySqlAuditor(auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }

    /// <summary>
    /// Apply the MySQL connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="server">The name of the server.</param>
    /// <param name="database">The name of te database for the initial connection.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string server, string database, string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new MySqlConnectionFactory(server, database);
        var auditor = new MySqlAuditor(auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }

    /// <summary>
    /// Apply the MySQL connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="server">The name of the server.</param>
    /// <param name="database">The name of te database for the initial connection.</param>
    /// <param name="userId">The user ID that should be used to connect with.</param>
    /// <param name="password">The password that should be used to make the connection.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string server, string database, string userId, string password, string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new MySqlConnectionFactory(server, database, userId, password);
        var auditor = new MySqlAuditor(auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }
}