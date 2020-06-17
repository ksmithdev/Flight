using Flight;
using Flight.Providers;
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

            //            // build a migration
            //            var migration = new MigrationBuilder()
            //                .UseSqlServer(@"(LocalDB)\MSSQLLocalDB", database: "MigrationTest", auditSchema: "Flight", auditTable: "ChangeSets")
            //                .UseTransaction()
            //#if DEBUG
            //                .AddInitializationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Initialization" }))
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { Recursive = true, Sorted = true })
            //#else
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { Sorted = true })
            //#endif
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Views" }) { Idempotent = true })
            //                .Build(loggerFactory);

            //            // execute the migration
            //            await migration.MigrateAsync();

            //            // build a migration
            //            var sqliteMigration = new MigrationBuilder()
            //                .UseSqlite("Data Source=:memory:;", auditTable: "changesets")
            //                .UseTransaction()
            //#if DEBUG
            //                .AddInitializationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Initialization" }))
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Recursive = true, Sorted = true })
            //#else
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Sorted = true })
            //#endif
            //                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Views" }) { Idempotent = true })
            //                .Build(loggerFactory);

            //            // execute the migration
            //            await sqliteMigration.MigrateAsync();

            // build a migration
            var postgreMigration = new MigrationBuilder()
                .UsePostgres(@"127.0.0.1", database: "migration_test", username: "postgres", password: "postgres", auditSchema: "flight", auditTable: "change_sets")
                .UseTransaction()
#if DEBUG
                .AddInitializationScripts(new FileSystemScriptProvider(new[] { @"Postgres\Initialization" }))
                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Postgres\Migrations" }) { Recursive = true, Sorted = true })
#else
                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Postgres\Migrations" }) { Sorted = true })
#endif
                .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Postgres\Views" }) { Idempotent = true })
                .Build(loggerFactory);

            // execute the migration
            await postgreMigration.MigrateAsync();
        }
    }
}