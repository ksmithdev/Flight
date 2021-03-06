﻿namespace Flight
{
    using Flight.Database;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SqlAuditor : AuditorBase
    {
        private readonly string schemaName;
        private readonly string tableName;

        public SqlAuditor(string schemaName, string tableName)
        {
            // TODO: check schemaName and tableName for invalid characters and throw exception to prevent a possible sql injection attack
            this.schemaName = schemaName;
            this.tableName = tableName;
        }

        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using (var command = connection.CreateCommand())
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

            using (var command = connection.CreateCommand())
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

        public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM [{schemaName}].[{tableName}]";

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

        public override async Task StoreEntryAsync(DbConnection connection, DbTransaction? transaction, IScript script, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = $"INSERT INTO [{schemaName}].[{tableName}] (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, GETUTCDATE(), CURRENT_USER)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}