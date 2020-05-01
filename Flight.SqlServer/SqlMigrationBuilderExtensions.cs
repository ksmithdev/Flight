namespace Flight
{
    using System;

    public static class SqlMigrationBuilderExtensions
    {
        public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string dataSource, string database, string auditSchema = "Flight", string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionManager = new SqlConnectionFactory(dataSource, database);
            var batchManager = new SqlBatchManager();
            var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

            migrationBuilder.SetConnectionFactory(connectionManager);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string connectionString, string auditSchema = "Flight", string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionManager = new SqlConnectionFactory(connectionString);
            var batchManager = new SqlBatchManager();
            var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

            migrationBuilder.SetConnectionFactory(connectionManager);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseSqlServer(this MigrationBuilder migrationBuilder, string dataSource, string userId, string password, string database, string auditSchema = "Flight", string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new SqlConnectionFactory(dataSource, database, userId, password);
            var batchManager = new SqlBatchManager();
            var auditor = new SqlAuditor(schemaName: auditSchema, tableName: auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }
    }
}