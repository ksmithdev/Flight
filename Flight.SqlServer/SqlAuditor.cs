namespace Flight;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Flight.Database;

/// <summary>
/// Represents an implementation of <see cref="IAuditor"/> for SQL Server connections.
/// </summary>
internal partial class SqlAuditor : AuditorBase
{
    private readonly string schemaName;
    private readonly string tableName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlAuditor"/> class.
    /// </summary>
    /// <param name="schemaName">The schema name for the audit table.</param>
    /// <param name="tableName">The table name for the audit table.</param>
    public SqlAuditor(string schemaName, string tableName)
    {
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaName, nameof(schemaName));
        ArgumentException.ThrowIfNullOrWhiteSpace(tableName, nameof(tableName));
#else
        if (string.IsNullOrWhiteSpace(schemaName))
        {
            throw new ArgumentNullException(nameof(schemaName));
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentNullException(nameof(tableName));
        }
#endif

        if (!ValidSchemaAndTableNameRegex().IsMatch(schemaName))
        {
            throw new ArgumentException(
                $"{schemaName} name contains invalid characters. Only letters, numbers, @, $, #, and _ allowed",
                nameof(schemaName));
        }

        if (!ValidSchemaAndTableNameRegex().IsMatch(tableName))
        {
            throw new ArgumentException(
                $"{tableName} name contains invalid characters. Only letters, numbers, @, $, #, and _ allowed",
                nameof(tableName));
        }

        this.schemaName = schemaName;
        this.tableName = tableName;
    }

    /// <inheritdoc/>
    public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        await using (var command = connection.CreateCommand())
#else
        using (var command = connection.CreateCommand())
#endif
        {
            command.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME=@schema;";
            command.AddParameter("@schema", schemaName);

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

            if (count == 0)
            {
                command.Parameters.Clear();
                command.CommandText = $"CREATE SCHEMA [{schemaName}]";
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        await using (var command = connection.CreateCommand())
#else
        using (var command = connection.CreateCommand())
#endif
        {
            command.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@schema AND TABLE_NAME=@table;";

            command.AddParameter("@schema", schemaName);
            command.AddParameter("@table", tableName);

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

            if (count == 0)
            {
                command.Parameters.Clear();
                command.CommandText = $@"CREATE TABLE [{schemaName}].[{tableName}] (
ScriptName nvarchar(255) NOT NULL,
Checksum nchar(44) NOT NULL,
Idempotent bit NOT NULL,
Applied datetime NOT NULL,
AppliedBy nvarchar(104) NOT NULL
);";

                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var auditEntries = new List<AuditEntry>();
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        await using var command = connection.CreateCommand();
#else
        using var command = connection.CreateCommand();
#endif
        command.CommandText = $"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM [{schemaName}].[{tableName}]";

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
#else
        using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
#endif
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            auditEntries.Add(new AuditEntry
            {
                ScriptName = reader.GetString(0),
                Checksum = reader.GetString(1),
                Idempotent = reader.GetBoolean(2),
                Applied = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc),
                AppliedBy = reader.GetString(4),
            });
        }

        return auditEntries;
    }

    /// <inheritdoc/>
    public override async Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
        await using var command = connection.CreateCommand();
#else
        using var command = connection.CreateCommand();
#endif
        command.Transaction = transaction;
        command.CommandText = $"INSERT INTO [{schemaName}].[{tableName}] (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, GETUTCDATE(), CURRENT_USER)";
        command.AddParameter("@scriptName", script.ScriptName);
        command.AddParameter("@checksum", script.Checksum);
        command.AddParameter("@idempotent", script.Idempotent);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
    [GeneratedRegex("^[a-zA-Z_@#][a-zA-Z0-9_@$#_]*$")]
    private static partial Regex ValidSchemaAndTableNameRegex();
#else
    private static Regex ValidSchemaAndTableNameRegex() => new(@"^[a-zA-Z_@#][a-zA-Z0-9_@$#_]*$");
#endif
}