namespace Flight.Providers;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flight.Logging;

/// <summary>
/// Represents a script provider that loads scripts from embedded resources.
/// </summary>
public class EmbeddedResourceScriptProvider : ScriptProviderBase
{
    private readonly IEnumerable<string> resourceNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceScriptProvider"/> class.
    /// </summary>
    /// <param name="resourceNames">The names of the embedded resources to load.</param>
    public EmbeddedResourceScriptProvider(IEnumerable<string> resourceNames)
    {
        this.resourceNames = resourceNames;
    }

    /// <summary>
    /// Gets or sets the filter string to match against the names of files in path. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
    /// </summary>
    public string Filter { get; set; } = "*.*";

    /// <summary>
    /// Gets or sets a value indicating whether the resources are treated as idempotent.
    /// </summary>
    public bool Idempotent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to sort the files returned from the provider.
    /// </summary>
    public bool Sorted { get; set; } = true;

    /// <inheritdoc/>
    public override IEnumerable<IScript> GetScripts()
    {
        try
        {
            Log.Trace($"Begin {nameof(EmbeddedResourceScriptProvider)}.{nameof(this.GetScripts)}");

            var scripts = new List<EmbeddedResourceScript>();

            Assembly assembly = Assembly.GetEntryAssembly();
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceNames.Any(n => resourceName.StartsWith(n, System.StringComparison.OrdinalIgnoreCase)))
                {
                    if (!PathMatcher.IsMatch(resourceName, Filter))
                    {
                        continue;
                    }

                    Log.Debug($"Loading script from embedded resource: {resourceName}");
                    scripts.Add(new EmbeddedResourceScript(resourceName, Idempotent));
                }
            }

            return Sorted ? scripts.OrderBy(s => s.ScriptName) : scripts;
        }
        finally
        {
            Log.Trace($"End {nameof(EmbeddedResourceScriptProvider)}.{nameof(this.GetScripts)}");
        }
    }
}