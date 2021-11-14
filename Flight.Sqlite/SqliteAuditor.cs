namespace Flight
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Globalization;
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
            // TODO: check auditTable for invalid characters and throw exception to prevent a possible sql injection attack
            this.auditTable = auditTable ?? throw new ArgumentNullException(nameof(auditTable));
        }

        /// <inheritdoc/>
        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE name=@table and type='table';";
            command.AddParameter("@table", this.auditTable);

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

            if (count == 0)
            {
                command.Parameters.Clear();
                command.CommandText = $@"CREATE TABLE ""{this.auditTable}"" (
    script_name TEXT     NOT NULL,
    checksum    TEXT     NOT NULL,
    idempotent  BOOLEAN  NOT NULL,
    applied     DATETIME NOT NULL,
    applied_by  TEXT     NOT NULL
);";
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT script_name, checksum, idempotent, applied, applied_by FROM ""{this.auditTable}""";

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
        public override async Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"INSERT INTO ""{this.auditTable}"" (script_name, checksum, idempotent, applied, applied_by) VALUES (@scriptName, @checksum, @idempotent, datetime('now'), @appliedBy)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);
            command.AddParameter("@appliedBy", Environment.UserName);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}