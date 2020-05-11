namespace Flight.Providers
{
    using Flight.Logging;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Defines a script provider that loads scripts from the file system.
    /// </summary>
    public class FileSystemScriptProvider : ScriptProviderBase
    {
        private readonly IEnumerable<string> locations;

        public FileSystemScriptProvider(IEnumerable<string> locations)
        {
            this.locations = locations;
        }

        /// <summary>
        /// Get or set the filter string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </summary>
        public string Filter { get; set; } = "*.sql";

        /// <summary>
        /// Get or set whether the files are treated as idempotent.
        /// </summary>
        public bool Idempotent { get; set; }

        /// <summary>
        /// Get or set whether to search for files recursively.
        /// </summary>
        public bool Recursive { get; set; }

        /// <summary>
        /// Get or set whether to sort the files returned from the provider.
        /// </summary>
        public bool Sorted { get; set; } = true;

        /// <summary>
        /// <inheritdoc cref="IScriptProvider.GetScripts"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IScript> GetScripts()
        {
            var scripts = new List<FileSystemScript>();

            var paths = locations.Select(Path.GetFullPath);

            foreach (var path in paths.Distinct())
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }

                Log.Debug($"Searching for scripts in {path}...");
                foreach (var filePath in Directory.GetFiles(path, Filter, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    Log.Debug($"Found {filePath}");
                    scripts.Add(new FileSystemScript(filePath, Idempotent));
                }
            }

            return Sorted ? scripts.OrderBy(s => s.ScriptName).AsEnumerable() : scripts;
        }
    }
}