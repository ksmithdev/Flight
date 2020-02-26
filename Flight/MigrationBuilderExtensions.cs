using Flight.Executors;
using System;

namespace Flight
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder UseNoTransaction(this MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            migrationBuilder.SetScriptExecutor(new NoTransactionExecutor());

            return migrationBuilder;
        }

        public static MigrationBuilder UseOneTransaction(this MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            migrationBuilder.SetScriptExecutor(new OneTransactionExecutor());

            return migrationBuilder;
        }

        public static MigrationBuilder UseTransactionPerScript(this MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            migrationBuilder.SetScriptExecutor(new TransactionPerScriptExecutor());

            return migrationBuilder;
        }
    }
}