namespace Flight;

using System;
using Microsoft.Data.Sqlite;

/// <summary>
/// The collection of SQLite extension methods for the <see cref="MigrationBuilder"/>.
/// </summary>
public static class SqliteMigrationBuilderExtensions
{
    /// <summary>
    /// Apply the SQLite connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="connectionString">The string used to open the connection.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The <see cref="MigrationBuilder"/> instance with the applied connection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string connectionString, string auditTable = "changesets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new SqliteConnectionFactory(connectionString);
        var auditor = new SqliteAuditor(auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }

    /// <summary>
    /// Apply the SQLite connection information to the migration.
    /// </summary>
    /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
    /// <param name="dataSource">The database file.</param>
    /// <param name="sqliteOpenMode">The connection mode.</param>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    /// <returns>The <see cref="MigrationBuilder"/> instance with the applied connection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
    public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string dataSource, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, string auditTable = "changesets")
    {
        if (migrationBuilder == null)
        {
            throw new ArgumentNullException(nameof(migrationBuilder));
        }

        var connectionFactory = new SqliteConnectionFactory(dataSource, sqliteOpenMode);
        var auditor = new SqliteAuditor(auditTable);

        migrationBuilder.SetConnectionFactory(connectionFactory);
        migrationBuilder.SetAuditor(auditor);

        return migrationBuilder;
    }
}