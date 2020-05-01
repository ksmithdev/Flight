Flight
========

Flight will solve your problem of where to start with documentation,
by providing a basic explanation of how to do it easily.

Look how easy it is to use:

```csharp
// generate a logger factory
using var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Trace);
});

// build a migration
var migration = new MigrationBuilder()
    // set the connection string to the target database
    // you can also specify the database and table to store the migration information
    .UseSqlServer(@"(LocalDB)\MSSQLLocalDB", database: "MigrationTest", auditSchema: "Flight", auditTable: "ChangeSets")
    // set the migration strategy
    // using one transaction will execute all migration scripts inside a transaction and roll them all back if there is a failure
    .UseOneTransaction()
// you can have multiple migration plans based on configuration
#if DEBUG
    // initialization scripts are run before the regular migration scripts. the audit log will be verified and created after this process. you can use this to reset a database to a known state for testing. these scripts are not executed inside of a transaction.
    .AddInitializationScripts(
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
    // idempotent scripts will be run every time the file changes
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

License
-------

The project is licensed under the MIT license.