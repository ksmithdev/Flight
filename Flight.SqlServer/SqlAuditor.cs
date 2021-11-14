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
    /// Represents an implementation of <see cref="IAuditor"/> for SQL Server connections.
    /// </summary>
    internal class SqlAuditor : AuditorBase
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
            // TODO: check schemaName and tableName for invalid characters and throw exception to prevent a possible sql injection attack
            this.schemaName = schemaName;
            this.tableName = tableName;
        }

        /// <inheritdoc/>
        public override async Task EnsureCreatedAsync(DbConnection connection, CancellationToken cancellationToken)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME=@schema;";
                command.AddParameter("@schema", this.schemaName);

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

                if (count == 0)
                {
                    command.Parameters.Clear();
                    command.CommandText = $"CREATE SCHEMA [{this.schemaName}]";
                    await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@schema AND TABLE_NAME=@table;";

                command.AddParameter("@schema", this.schemaName);
                command.AddParameter("@table", this.tableName);

                var result = await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                var count = Convert.ToInt32(result, CultureInfo.InvariantCulture);

                if (count == 0)
                {
                    command.Parameters.Clear();
                    command.CommandText = $@"CREATE TABLE [{this.schemaName}].[{this.tableName}] (
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

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT ScriptName, Checksum, Idempotent, Applied, AppliedBy FROM [{this.schemaName}].[{this.tableName}]";

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
            command.CommandText = $"INSERT INTO [{this.schemaName}].[{this.tableName}] (ScriptName, Checksum, Idempotent, Applied, AppliedBy) VALUES (@scriptName, @checksum, @idempotent, GETUTCDATE(), CURRENT_USER)";
            command.AddParameter("@scriptName", script.ScriptName);
            command.AddParameter("@checksum", script.Checksum);
            command.AddParameter("@idempotent", script.Idempotent);

            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}