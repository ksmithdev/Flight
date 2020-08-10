namespace Flight
{
    using Flight.Database;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    internal class MySqlAuditor : AuditorBase
    {
        private readonly string tableName;

        public MySqlAuditor(string tableName)
        {
            // TODO: check tableName for invalid characters and throw exception to prevent a possible sql injection attack
            this.tableName = tableName;
        }

        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $@"CREATE TABLE IF NOT EXISTS {tableName} (
    ScriptName VARCHAR(255) NOT NULL,
    Checksum CHAR(44) NOT NULL,
    Idempotent BIT NOT NULL,
    Applied DATETIME NOT NULL,
    AppliedBy VARCHAR(104) NOT NULL
);";

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task<IEnumerable<AuditEntry>> LoadEntriesAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            var auditEntries = new List<AuditEntry>();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM {tableName}";

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
            command.CommandText = $"INSERT INTO {tableName} (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, UTC_TIMESTAMP(), USER())";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}