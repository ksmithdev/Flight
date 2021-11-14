namespace Flight
{
    using System;

    /// <summary>
    /// The collection of Postgres extension methods for the <see cref="MigrationBuilder"/>.
    /// </summary>
    public static class PostgresMigrationBuilderExtensions
    {
        /// <summary>
        /// Apply the PostgreSQL connection information to the migration.
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
        /// <param name="connectionString">The connection used to open the PostgreSQL database.</param>
        /// <param name="auditSchema">The name of the audit table schema.</param>
        /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
        /// <returns>The migration builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
        public static MigrationBuilder UsePostgres(this MigrationBuilder migrationBuilder, string connectionString, string auditSchema = "flight", string auditTable = "change_sets")
        {
            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            var connectionFactory = new PostgresConnectionFactory(connectionString);
            var auditor = new PostgresAuditor(auditSchema, auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        /// <summary>
        /// Apply the PostgreSQL connection information to the migration.
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
        /// <param name="host">The hostname or IP address of the PostgreSQL server to connect to.</param>
        /// <param name="database">The PostgreSQL database to connect to.</param>
        /// <param name="auditSchema">The name of the audit table schema.</param>
        /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
        /// <returns>The migration builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
        public static MigrationBuilder UsePostgres(this MigrationBuilder migrationBuilder, string host, string database, string auditSchema = "flight", string auditTable = "change_sets")
        {
            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            var connectionFactory = new PostgresConnectionFactory(host, database);
            var auditor = new PostgresAuditor(auditSchema, auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }

        /// <summary>
        /// Apply the PostgreSQL connection information to the migration.
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="MigrationBuilder"/> instance.</param>
        /// <param name="host">The hostname or IP address of the PostgreSQL server to connect to.</param>
        /// <param name="database">The PostgreSQL database to connect to.</param>
        /// <param name="username">The username to connect with.</param>
        /// <param name="password">The password to connect with.</param>
        /// <param name="auditSchema">The name of the audit table schema.</param>
        /// <param name="auditTable">The name of the audit table used for tracking applied migrations.</param>
        /// <returns>The migration builder instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <see cref="MigrationBuilder"/> parameter is null.</exception>
        public static MigrationBuilder UsePostgres(this MigrationBuilder migrationBuilder, string host, string database, string username, string password, string auditSchema = "flight", string auditTable = "change_sets")
        {
            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            var connectionFactory = new PostgresConnectionFactory(host, database, username, password);
            var auditor = new PostgresAuditor(auditSchema, auditTable);

            migrationBuilder.SetConnectionFactory(connectionFactory);
            migrationBuilder.SetAuditor(auditor);

            return migrationBuilder;
        }
    }
}