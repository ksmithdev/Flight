using Flight.Providers;
using Flight.Stages;
using System;

namespace Flight
{
    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder AddNoTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            return migrationBuilder.AddStage(() =>
                new NoTransactionStage(
                    new SortByScriptNameDecorator(scriptProvider)
                )
            );
        }

        public static MigrationBuilder AddOneTransactionStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            return migrationBuilder.AddStage(() =>
                new OneTransactionStage(
                    new SortByScriptNameDecorator(scriptProvider)
                )
            );
        }

        public static MigrationBuilder AddTransactionPerScriptStage(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            return migrationBuilder.AddStage(() =>
                new TransactionPerScriptStage(
                    new SortByScriptNameDecorator(scriptProvider)
                )
            );
        }
    }
}