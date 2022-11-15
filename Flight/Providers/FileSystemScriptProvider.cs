namespace Flight.Providers;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Flight.Logging;

/// <summary>
/// Represents a script provider that loads scripts from the file system.
/// </summary>
public class FileSystemScriptProvider : ScriptProviderBase
{
    private readonly IEnumerable<string> locations;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemScriptProvider"/> class.
    /// </summary>
    /// <param name="locations">The relative location of the script files.</param>
    public FileSystemScriptProvider(IEnumerable<string> locations)
    {
        this.locations = locations;
    }

    /// <summary>
    /// Gets or sets the filter string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
    /// </summary>
    public string Filter { get; set; } = "*.sql";

    /// <summary>
    /// Gets or sets a value indicating whether the files are treated as idempotent.
    /// </summary>
    public bool Idempotent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to search for files recursively.
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to sort the files returned from the provider.
    /// </summary>
    public bool Sorted { get; set; } = true;

    /// <inheritdoc/>
    public override IEnumerable<IScript> GetScripts()
    {
        try
        {
            Log.Trace($"Begin {nameof(FileSystemScriptProvider)}.{nameof(this.GetScripts)}");

            var scripts = new List<FileSystemScript>();

            var paths = this.locations.Select(Path.GetFullPath);

            foreach (var path in paths.Distinct())
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                Log.Debug($"Searching for scripts in {path}...");
                foreach (var filePath in Directory.GetFiles(path, this.Filter, this.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    Log.Debug($"Found {filePath}");
                    scripts.Add(new FileSystemScript(filePath, this.Idempotent));
                }
            }

            return this.Sorted ? scripts.OrderBy(s => s.ScriptName).AsEnumerable() : scripts;
        }
        finally
        {
            Log.Trace($"End {nameof(FileSystemScriptProvider)}.{nameof(this.GetScripts)}");
        }
    }
}