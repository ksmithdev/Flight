using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Flight.Providers
{
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
            Logger.LogDebug($"Search settings: recusive={Recursive}, filter={Filter}");

            var scripts = new List<FileSystemScript>();

            var paths = locations.Select(Path.GetFullPath);

            foreach (var path in paths.Distinct())
            {
                if (!Directory.Exists(path))
                {
                    Logger.LogWarning($"Location does not exist: {path}");
                    continue;
                }

                foreach (var filePath in Directory.GetFiles(path, Filter, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                    scripts.Add(new FileSystemScript(filePath, Idempotent));
            }

            return Sorted ? scripts.OrderBy(s => s.ScriptName).AsEnumerable() : scripts;
        }
    }
}