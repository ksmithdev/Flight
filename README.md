Flight
========

Flight is an easy to use framework for applying SQL to a database deterministically. You create a migration plan and Flight will take care of the rest. 

Look how easy it is to use:

~~~csharp
// generate a logger factory
using var loggerFactory = LoggerFactory.Create(b =>
{
    b.AddConsole();
    b.SetMinimumLevel(LogLevel.Trace);
});

// build a migration
var migration = new MigrationBuilder()
    // set the connection string to the target database
    .UseSqlServer(@"(LocalDB)\MSSQLLocalDB", database: "MigrationTest")
    // using one transaction will execute all migration scripts inside a transaction
    .UseOneTransaction()
    // scripts can be loaded from the file system and will be only executed once
    .AddMigrationScripts(
        new FileSystemScriptProvider(new[] { @"SqlServer\Migrations" }) { 
            Recursive = true, // search all subdirectories 
            Sorted = true }) // sort the scripts by filename prior to execution
    // idempotent scripts will be run when the file changes
    // this is great for views, stored procedures, and function
    .AddMigrationScripts(
        new FileSystemScriptProvider(new[] { @"SqlServer\Views" }) { 
            Idempotent = true })
    .Build(loggerFactory);

// begin the migration
await migration.MigrateAsync();
~~~

Supported Databases
--------

- [x] SQL Server
- [x] SQLite
- [x] PostgreSQL
- [ ] MySQL

Installation
------------

Install Flight by running:

~~~powershell
# for MS SQL Server
dotnet add package KSmithDev.Flight.SqlServer
# for Sqlite
dotnet add package KSmithDev.Flight.Sqlite
# for PostgreSQL
dotnet add package KSmithDev.Flight.Postgres
~~~

Contribute
----------

- Issue Tracker: github.com/ksmithdev/Flight/issues
- Source Code: github.com/ksmithdev/Flight

License
-------

The project is licensed under the Apache License 2.0.
