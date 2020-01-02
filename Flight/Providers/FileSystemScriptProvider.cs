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

        public bool Idempotent { get; set; } = false;

        public bool Recursive { get; set; } = false;

        public override IEnumerable<IScript> GetScripts()
        {
            Logger.LogDebug($"Search settings: recusive={Recursive}, filter={Filter}");

            var scripts = new List<FileSystemScript>();

            foreach (var location in locations.Distinct())
            {
                if (!Directory.Exists(location))
                {
                    Logger.LogWarning($"Location does not exist: {location}");
                    continue;
                }

                var filePaths = Directory.GetFiles(location, Filter, Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                foreach (var filePath in filePaths)
                    scripts.Add(new FileSystemScript(filePath, Idempotent));
            }

            return scripts;
        }
    }
}