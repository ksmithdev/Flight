using Flight.Providers;
using Flight.Stages;
using System;

namespace Flight
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder AddNoTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            if (migrationBuilder is null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            return migrationBuilder.AddStage(new NoTransactionStage(scriptProvider));
        }

        public static MigrationBuilder AddOneTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            if (migrationBuilder is null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            return migrationBuilder.AddStage(new OneTransactionStage(scriptProvider));
        }

        public static MigrationBuilder AddTransactionPerScriptStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            if (migrationBuilder is null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            return migrationBuilder.AddStage(new TransactionPerScriptStage(scriptProvider));
        }
    }
}