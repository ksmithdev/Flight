﻿using Microsoft.Data.Sqlite;
using System;

namespace Flight
{
    public static class SqliteMigrationBuilderExtensions
    {
        public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string connectionString, string auditTable = "changesets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new SqliteConnectionFactory(connectionString);
            var batchManager = new SqliteBatchManager();
            var auditor = new SqliteAuditLog(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditLog(auditor);

            return migrationBuilder;
        }

        public static MigrationBuilder UseSqlite(this MigrationBuilder migrationBuilder, string dataSource, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, string auditTable = "changesets")
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            var connectionFactory = new SqliteConnectionFactory(dataSource, sqliteOpenMode);
            var batchManager = new SqliteBatchManager();
            var auditor = new SqliteAuditLog(auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetBatchManager(batchManager);
            migrationBuilder.SetAuditLog(auditor);

            return migrationBuilder;
        }
    }
}