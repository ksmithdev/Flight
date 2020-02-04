using Flight.Auditing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Flight
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
    internal class SqliteAuditLog : AuditLogBase
    {
        private readonly string auditTable;

        public SqliteAuditLog(string auditTable)
        {
            // TODO: check auditTable for invalid characters and throw exception to prevent a possible sql injection attack
            this.auditTable = auditTable ?? throw new ArgumentNullException(nameof(auditTable));
        }

        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE name=@table and type='table';";
            command.AddParameter("@table", auditTable);

            var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
            var count = Convert.ToInt32(result);

            if (count == 0)
            {
                command.Parameters.Clear();
                command.CommandText = $@"CREATE TABLE ""{auditTable}"" (
    script_name TEXT     NOT NULL,
    checksum    TEXT     NOT NULL,
    idempotent  BOOLEAN  NOT NULL,
    applied     DATETIME NOT NULL,
    applied_by  TEXT     NOT NULL
);";
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public override async Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"INSERT INTO ""{auditTable}"" (script_name, checksum, idempotent, applied, applied_by) VALUES (@scriptName, @checksum, @idempotent, datetime('now'), @appliedBy)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);
            command.AddParameter("@appliedBy", Environment.UserName);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        protected override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT script_name, checksum, idempotent, applied, applied_by FROM ""{auditTable}""";

            using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                auditEntries.Add(new AuditEntry
                {
                    ScriptName = reader.GetString(0),
                    Checksum = reader.GetString(1),
                    Idempotent = reader.GetBoolean(2),
                    Applied = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc),
                    AppliedBy = reader.GetString(4)
                });
            }

            return auditEntries;
        }
    }
}