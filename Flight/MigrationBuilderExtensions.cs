namespace Flight
{
    using System;
    using Flight.Providers;
    using Flight.Stages;

    public static class MigrationBuilderExtensions
    {
        public static MigrationBuilder AddStageInTransaction(this MigrationBuilder migrationBuilder, IScriptProvider scriptProvider)
        {
            if (migrationBuilder == null)
                throw new ArgumentNullException(nameof(migrationBuilder));

            return migrationBuilder.AddMigrationStage(
                new TransactionStage(scriptProvider)
            );
        }
    }
}