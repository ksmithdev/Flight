namespace Flight;

using System;

/// <summary>
/// The collection of SQL Server extension methods for the <see cref="MigrationBuilder"/>.
/// </summary>
public static class SqlMigrationBuilderExtensions
{
    /// <summary>
    /// Apply the SQL Server connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="dataSource">The name or network address of the instance of SQL Server to connect to.</param>
    /// <param name="database">The name of the database associated with the connection.</param>
    /// <param name="auditSchema">The name of the audit table schema.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string dataSource, string database, string auditSchema = "Flight", string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionManager = new SqlConnectionFactory(dataSource, database);
        var batchManager = new SqlBatchManager();
        var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

        migrationBuilder.SetConnectionFactory(connectionManager);
        migrationBuilder.SetBatchManager(batchManager);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }

    /// <summary>
    /// Apply the SQL Server connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="connectionString">The connection used to open the SQL Server database.</param>
    /// <param name="auditSchema">The name of the audit table schema.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string connectionString, string auditSchema = "Flight", string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionManager = new SqlConnectionFactory(connectionString);
        var batchManager = new SqlBatchManager();
        var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

        migrationBuilder.SetConnectionFactory(connectionManager);
        migrationBuilder.SetBatchManager(batchManager);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }

    /// <summary>
    /// Apply the SQL Server connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="dataSource">The name or network address of the instance of SQL Server to connect to.</param>
    /// <param name="userId">The user ID to use when connecting to SQL Server.</param>
    /// <param name="password">The password for the SQL Server account.</param>
    /// <param name="database">The name of the database associated with the connection.</param>
    /// <param name="auditSchema">The name of the audit table schema.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The migration builder instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string dataSource, string userId, string password, string database, string auditSchema = "Flight", string auditTable = "ChangeSets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new SqlConnectionFactory(dataSource, database, userId, password);
        var batchManager = new SqlBatchManager();
        var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetBatchManager(batchManager);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }
}