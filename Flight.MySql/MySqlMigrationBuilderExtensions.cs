namespace Flight
{
    using System;

    public static class MySqlMigrationBuilderExtensions
    {
        public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string connectionString, string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new MySqlConnectionFactory(connectionString);
            var batchManager = new MySqlBatchManager();
            var auditor = new MySqlAuditor(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string server, string database, string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new MySqlConnectionFactory(server, database);
            var batchManager = new MySqlBatchManager();
            var auditor = new MySqlAuditor(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseMySql(this MigrationBuilder migrationBuilder, string server, string database, string userId, string password, string auditTable = "ChangeSets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new MySqlConnectionFactory(server, database, userId, password);
            var batchManager = new MySqlBatchManager();
            var auditor = new MySqlAuditor(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }
    }
}