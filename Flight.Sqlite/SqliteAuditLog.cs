using Flight.Auditing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
            bool generateTable = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE name=@table and type='table';";
                command.AddParameter("@table", auditTable);

                var auditCount = await command.ExecuteScalarAsync(cancellationToken);

                generateTable = auditCount.Equals(0L);
            }

            if (generateTable)
            {
                using var command = connection.CreateCommand();
                command.CommandText = $@"CREATE TABLE ""{auditTable}"" (
    script_name TEXT     NOT NULL,
    checksum    TEXT     NOT NULL,
    idempotent  BOOLEAN  NOT NULL,
    applied     DATETIME NOT NULL,
    applied_by  TEXT     NOT NULL
);";
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public override async Task LogAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"INSERT INTO ""{auditTable}"" (script_name, checksum, idempotent, applied, applied_by) VALUES (@scriptName, @checksum, @idempotent, datetime('now'), @appliedBy)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);
            command.AddParameter("@appliedBy", Environment.UserName);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        protected override async Task<ILookup<string, AuditEntry>> LoadAuditEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT script_name, checksum, idempotent, applied, applied_by FROM ""{auditTable}""";

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
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

            return auditEntries.OrderByDescending(e => e.Applied).ToLookup(e => e.ScriptName, StringComparer.OrdinalIgnoreCase);
        }
    }
}