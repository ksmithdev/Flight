using Flight.Providers;
using Flight.Stages;

namespace Flight
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder AddNoTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider) => migrationBuilder.AddStage(new NoTransactionStage(scriptProvider));

        public static MigrationBuilder AddOneTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider) => migrationBuilder.AddStage(new OneTransactionStage(scriptProvider));

        public static MigrationBuilder AddTransactionPerScriptStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider) => migrationBuilder.AddStage(new TransactionPerScriptStage(scriptProvider));
    }
}