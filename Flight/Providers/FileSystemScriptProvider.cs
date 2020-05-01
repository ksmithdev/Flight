namespace Flight.Providers
{
    using Flight.Logging;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class FileSystemScriptProvider : ScriptProviderBase
    {
        private readonly IEnumerable<string> locations;

        public FileSystemScriptProvider(IEnumerable<string> locations)
        {
            this.locations = locations;
        }

        public string Filter { get; set; } = "*.sql";

        public bool Idempotent { get; set; }

        public bool Recursive { get; set; }

        public bool Sorted { get; set; } = true;

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