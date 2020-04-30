Flight
========

Flight will solve your problem of where to start with documentation,
by providing a basic explanation of how to do it easily.

Look how easy it is to use:

```csharp
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
    .UseOneTransaction()
// you can have multiple migration plans based on configuration
#if DEBUG
    // pre-migration scripts are run before the regular migration scripts. the audit log will be verified and created after this process. you can use this to reset a database to a known state for testing.
    .AddPreMigrationScripts(
        new FileSystemScriptProvider(new[] { @"SqlServer\Initialization" }))
    // scripts can be loaded from the file system and executed in a sorted manner based on the file name
    .AddMigrationScripts(
        new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { 
            Recursive = true, 
            Sorted = true })
#else
    .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) {
        Sorted = true })
#endif
    // idempotent scripts will be run every time they change
    .AddMigrationScripts(new FileSystemScriptProvider(new[] { @"SqlServer\Views" }) { 
        Idempotent = true })
    .Build(loggerFactory);

// begin the migration
await migration.MigrateAsync();
```

Features
--------

- Be awesome
- Make things faster

Installation
------------

Install Flight by running:

    install project

Contribute
----------

- Issue Tracker: github.com/animusblue/Flight/issues
- Source Code: github.com/animusblue/Flight

Support
-------

If you are having issues, please let us know.

License
-------

The project is licensed under the MIT license.