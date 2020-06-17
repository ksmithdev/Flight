namespace Flight
{
    using Flight.Database;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;

    internal class PostgresAuditor : AuditorBase
    {
        private readonly string schemaName;
        private readonly string tableName;

        public PostgresAuditor(string schemaName, string tableName)
        {
            // TODO: check schemaName and tableName for invalid characters and throw exception to prevent a possible sql injection attack
            this.schemaName = schemaName;
            this.tableName = tableName;
        }

        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM pg_catalog.pg_tables WHERE schemaname=@schema;";
                command.AddParameter("@schema", schemaName);

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

                if (count == 0)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"CREATE SCHEMA [{schemaName}];";
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM pg_catalog.pg_tables WHERE schemaname=@schema AND tablename=@table;";

                command.AddParameter("@schema", schemaName);
                command.AddParameter("@table", tableName);

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

                if (count == 0)
                {
                    command.Parameters.Clear();
                    command.CommandText = $@"CREATE TABLE [{schemaName}].[{tableName}] (
    script_name varchar(255) NOT NULL,
    checksum char(44) NOT NULL,
    idempotent bool NOT NULL,
    applied timestamptz NOT NULL,
    applied_by varchar(104) NOT NULL
);";

                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT script_name, checksum, idempotent, applied, applied_by FROM [{schemaName}].[{tableName}]";

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
            command.CommandText = $"INSERT INTO [{schemaName}].[{tableName}] (script_name, checksum, idempotent, applied, applied_by) VALUES (@scriptName, @checksum, @idempotent, now(), current_user())";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}