using Microsoft.Data.Sqlite;

namespace Flight
{
    public static class SqliteMigrationBuilderExtensions
    {
        public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string connectionString, string auditTable = "changesets")
        {
            var connectionFactory = new SqliteConnectionFactory(connectionString);
            var batchManager = new SqliteBatchManager();
            var auditor = new SqliteAuditLog(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string dataSource, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, string auditTable = "changesets")
        {
            var connectionFactory = new SqliteConnectionFactory(dataSource, sqliteOpenMode);
            var batchManager = new SqliteBatchManager();
            var auditor = new SqliteAuditLog(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }
    }
}