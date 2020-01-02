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
    internal class SqlAuditLog : AuditLogBase
    {
        private readonly string schemaName;
        private readonly string tableName;

        public SqlAuditLog(string schemaName, string tableName)
        {
            this.schemaName = schemaName;
            this.tableName = tableName;
        }

        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            bool generateSchema = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"SELECT COUNT(1) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME=@schema;";
                command.AddParameter("@schema", schemaName);

                var count = await command.ExecuteScalarAsync(cancellationToken);

                generateSchema = count.Equals(0);
            }

            if (generateSchema)
            {
                using var command = connection.CreateCommand();
                command.CommandText = $@"CREATE SCHEMA [{schemaName}]";
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            bool generateTable = false;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@schema AND TABLE_NAME=@table;";

                command.AddParameter("@schema", schemaName);
                command.AddParameter("@table", tableName);

                var count = await command.ExecuteScalarAsync(cancellationToken);

                generateTable = count.Equals(0);
            }

            if (generateTable)
            {
                using var command = connection.CreateCommand();
                command.CommandText = $@"CREATE TABLE [{schemaName}].[{tableName}] (
    ScriptName nvarchar(255) NOT NULL,
    Checksum nvarchar(25) NOT NULL,
    Idempotent bit NOT NULL,
    Applied datetime NOT NULL,
    AppliedBy nvarchar(104) NOT NULL
);";

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public override async Task LogAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $@"INSERT INTO [{schemaName}].[{tableName}] (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, GETUTCDATE(), CURRENT_USER)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        protected override async Task<ILookup<string, AuditEntry>> LoadAuditEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $@"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM [{schemaName}].[{tableName}]";
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