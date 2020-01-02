using System;
using System.Threading.Tasks;
using Flight;
using Flight.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlightSample
{
    internal class Program
    {
        public static IConfiguration Configuration { get; private set; }

        private static async Task Main(string[] _)
        {
            Console.WriteLine("Welcome to the Flight Sample App!");

            var configurationBuilder = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("FLIGHT_ENVIRONMENT") ?? "debug"}.json", optional: true, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            using var loggerFactory = LoggerFactory.Create(b =>
            {
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Trace);
            });

            var migration = new MigrationBuilder()
                .UseSqlServer(@"(LocalDB)\MSSQLLocalDB", database: "MigrationTest", auditSchema: "Flight", auditTable: "ChangeSets")
#if DEBUG
                .InitializeDatabase(new FileSystemScriptProvider(new[] { @"SqlServer\Initialization" }))
                .AddOneTransactionStage(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { Recursive = true })
#else
                .AddOneTransactionStage(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }))
#endif
                .AddTransactionPerScriptStage(new FileSystemScriptProvider(new[] { @"SqlServer\Views" }) { Idempotent = true })
                .Build(loggerFactory);

            await migration.MigrateAsync();

            var sqliteMigration = new MigrationBuilder()
                .UseSqlite(@"Data Source=MigrationTest.sqlite;", auditTable: "changesets")
#if DEBUG
                .InitializeDatabase(new FileSystemScriptProvider(new[] { @"Sqlite\Initialization" }))
                .AddOneTransactionStage(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Recursive = true })
#else
                .AddOneTransactionStage(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }))
#endif
                .AddTransactionPerScriptStage(new FileSystemScriptProvider(new[] { @"Sqlite\Views" }) { Idempotent = true })
                .Build(loggerFactory);

            await sqliteMigration.MigrateAsync();
        }
    }
}
