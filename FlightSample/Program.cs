using System;
using System.Threading.Tasks;
using Flight;
using Flight.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FlightSample;

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
        var sqliteMigration = new MigrationBuilder()
            .UseSqlite("Data Source=:memory:;", auditTable: "changesets")
            .UseTransaction()
#if DEBUG
            .AddInitializationScripts(new FileSystemScriptProvider([@"Sqlite\Initialization"]))
            .AddMigrationScripts(new FileSystemScriptProvider([@"Sqlite\Migrations"]) { Recursive = true, Sorted = true })
            .AddMigrationScripts(new EmbeddedResourceScriptProvider([@"FlightSample.Embedded.Sqlite"]) { Filter = "*.sql", Sorted = true })
#else
            .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"Sqlite\Migrations" }) { Sorted = true })
#endif
            .AddMigrationScripts(new FileSystemScriptProvider([@"Sqlite\Views"]) { Idempotent = true })
            .Build(loggerFactory);

        // execute the migration
        await sqliteMigration.MigrateAsync();
    }
}