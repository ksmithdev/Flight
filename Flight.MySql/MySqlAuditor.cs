namespace Flight;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Flight.Database;

/// <summary>
/// Represents an implementation of <see cref="IAuditor"/> for MySQL connections.
/// </summary>
internal class MySqlAuditor : AuditorBase
{
    private readonly string tableName;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlAuditor"/> class.
    /// </summary>
    /// <param name="tableName">The name of the audit table used to store the applied scripts.</param>
    public MySqlAuditor(string tableName)
    {
        // TODO: check tableName for invalid characters and throw exception to prevent a possible sql injection attack
        this.tableName = tableName;
    }

    /// <inheritdoc/>
    public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $@"CREATE TABLE IF NOT EXISTS {this.tableName} (
    ScriptName VARCHAR(255) NOT NULL,
    Checksum CHAR(44) NOT NULL,
    Idempotent BIT NOT NULL,
    Applied DATETIME NOT NULL,
    AppliedBy VARCHAR(104) NOT NULL
);";

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var auditEntries = new List<AuditEntry>();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM {this.tableName}";

        using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
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
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"INSERT INTO {this.tableName} (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, UTC_TIMESTAMP(), USER())";
        command.AddParameter("@scriptName", script.ScriptName);
        command.AddParameter("@checksum", script.Checksum);
        command.AddParameter("@idempotent", script.Idempotent);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}