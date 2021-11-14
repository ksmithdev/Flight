namespace Flight.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Represents a script provider for embedded script powers.
    /// </summary>
    public class EmbeddedScriptProvider : IScriptProvider
    {
        private readonly Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScriptProvider"/> class.
        /// </summary>
        /// <param name="assembly">The assembly to search for embedded script files.</param>
        public EmbeddedScriptProvider(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
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
        public IEnumerable<IScript> GetScripts()
        {
            foreach (var resource in this.assembly.GetManifestResourceNames())
            {
                var components = resource.Split('.');

                if (System.IO.Path.GetExtension(resource) != ".sql")
                {
                    continue;
                }

                yield return new EmbeddedScript(resource, this.assembly.GetManifestResourceStream(resource), this.Idempotent);
            }
        }

        private class Entry
        {
            public string Path { get; set; } = string.Empty;

            public HashSet<Entry> Entries { get; } = new HashSet<Entry>();

            public HashSet<string> Resources { get; } = new HashSet<string>();
        }
    }
}