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
/// Represents the auditor for SQLite databases.
/// </summary>
internal class SqliteAuditor : AuditorBase
{
    private readonly string auditTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteAuditor"/> class.
    /// </summary>
    /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
    public SqliteAuditor(string auditTable)
    {
        if (string.IsNullOrWhiteSpace(auditTable))
        {
            throw new ArgumentNullException(nameof(auditTable));
        }

        if (!ValidTableNameRegex().IsMatch(auditTable))
        {
            throw new ArgumentException(
                $"{auditTable} name contains invalid characters. Only letters, numbers, and underscores allowed (also cannot start with 'sqlite_')",
                nameof(auditTable));
        }

        this.auditTable = auditTable;
    }

    /// <inheritdoc/>
    public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
    {
#if NETSTANDARD2_1_OR_GREATER
        await using var command = connection.CreateCommand();
#else
        using var command = connection.CreateCommand();
#endif
        command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE name=@table and type='table';";
        command.AddParameter("@table", auditTable);

        var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
        var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

        if (count == 0)
        {
            command.Parameters.Clear();
            command.CommandText = $"""
CREATE TABLE "{auditTable}" (
    script_name TEXT     NOT NULL,
    checksum    TEXT     NOT NULL,
    idempotent  BOOLEAN  NOT NULL,
    applied     DATETIME NOT NULL,
    applied_by  TEXT     NOT NULL
);
""";
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var auditEntries = new List<AuditEntry>();

#if NETSTANDARD2_1_OR_GREATER
        await using var command = connection.CreateCommand();
#else
        using var command = connection.CreateCommand();
#endif
        command.CommandText = $@"SELECT script_name, checksum, idempotent, applied, applied_by FROM ""{auditTable}""";

#if NETSTANDARD2_1_OR_GREATER
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
    public override async Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_1_OR_GREATER
        await using var command = connection.CreateCommand();
#else
        using var command = connection.CreateCommand();
#endif
        command.Transaction = transaction;
        command.CommandText = $@"INSERT INTO ""{auditTable}"" (script_name, checksum, idempotent, applied, applied_by) VALUES (@scriptName, @checksum, @idempotent, datetime('now'), @appliedBy)";
        command.AddParameter("@scriptName", script.ScriptName);
        command.AddParameter("@checksum", script.Checksum);
        command.AddParameter("@idempotent", script.Idempotent);
        command.AddParameter("@appliedBy", Environment.UserName);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Regex ValidTableNameRegex() => new(@"^(?!sqlite_)[a-zA-Z_][a-zA-Z0-9_]*$");
}