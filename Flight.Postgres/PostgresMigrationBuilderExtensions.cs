namespace Flight
{
    using System;

    public static class PostgresMigrationBuilderExtensions
    {
        public static MigrationBuilder UsePostgres(this MigrationBuilder migrationBuilder, string connectionString, string auditSchema = "flight", string auditTable = "change_sets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new PostgresConnectionFactory(connectionString);
            var batchManager = new PostgresBatchManager();
            var auditor = new PostgresAuditor(auditSchema, auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UsePostgres(this MigrationBuilder migrationBuilder, string host, string database, string auditSchema = "flight", string auditTable = "change_sets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new PostgresConnectionFactory(host, database);
            var batchManager = new PostgresBatchManager();
            var auditor = new PostgresAuditor(auditSchema, auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }
    }
}