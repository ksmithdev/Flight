using Flight;
using Flight.Providers;
using Flight.Stages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FlightSample
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; }

        private static async Task Main(string[] _)
        {
            Console.WriteLine("Welcome to the Flight Sample App!");

            // load any application specific configuration
            var configurationBuilder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("FLIGHT_ENVIRONMENT") ?? "Debug"}.json", optional: true, reloadOnChange: true);
            Configuration = configurationBuilder.Build();

            // generate a logger factory
            using var loggerFactory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Trace);
            });

            // build a migration
            var migration = new MigrationBuilder()
                .UseSqlServer(@"(LocalDB)\MSSQLLocalDB", database: "MigrationTest", auditSchema: "Flight", auditTable: "ChangeSets")
#if DEBUG
                .AddInitializationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Initialization" }))
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { Recursive = true, Sorted = true }))
#else
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { Sorted = true }))
#endif
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"SqlServer\Views" }) { Idempotent = true }))
                .Build(loggerFactory);

            // begin the migration
            await migration.MigrateAsync();

            var sqliteMigration = new MigrationBuilder()
                .UseSqlite("Data Source=:memory:;", auditTable: "changesets")
#if DEBUG
                .AddInitializationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Initialization" }))
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Recursive = true, Sorted = true }))
#else
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Sorted = true }))
#endif
                .AddMigrationStage(new TransactionStage(new FileSystemScriptProvider(new[] { @"Sqlite\Views" }) { Idempotent = true }))
                .Build(loggerFactory);

            await sqliteMigration.MigrateAsync();
        }
    }
}