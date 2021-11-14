namespace Flight.Providers
{
    using System.Collections.Generic;
    using Flight.Logging;

    /// <summary>
    /// Represents a composite script provider to group a collection of script providers into a single object.
    /// </summary>
    public class CompositeScriptProvider : ScriptProviderBase
    {
        private readonly ICollection<IScriptProvider> scriptProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeScriptProvider"/> class.
        /// </summary>
        public CompositeScriptProvider()
        {
            this.scriptProviders = new List<IScriptProvider>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeScriptProvider"/> class.
        /// </summary>
        /// <param name="scriptProviders">The collection of script providers.</param>
        public CompositeScriptProvider(params IScriptProvider[] scriptProviders)
        {
            this.scriptProviders = new List<IScriptProvider>(scriptProviders);
        }

        /// <summary>
        /// Add the supplied provider to the composite collection.
        /// </summary>
        /// <param name="scriptProvider">The script provider to add.</param>
        public void AddScriptProvider(IScriptProvider scriptProvider) => this.scriptProviders.Add(scriptProvider);

        /// <summary>
        /// <inheritdoc cref="IScriptProvider.GetScripts"/>
        /// </summary>
        /// <returns>The collection of scripts to execute.</returns>
        public override IEnumerable<IScript> GetScripts()
        {
            try
            {
                Log.Trace($"Begin {nameof(CompositeScriptProvider)}.{nameof(this.GetScripts)}");

                foreach (var scriptProvider in this.scriptProviders)
                {
                    foreach (var script in scriptProvider.GetScripts())
                    {
                        yield return script;
                    }
                }
            }
            finally
            {
                Log.Trace($"End {nameof(CompositeScriptProvider)}.{nameof(this.GetScripts)}");
            }
        }

        /// <summary>
        /// Remove the supplied provier from the composite collection.
        /// </summary>
        /// <param name="scriptProvider">The script provider to remove.</param>
        /// <returns>Whether the script provider was removed.</returns>
        public bool RemoveScriptProvider(IScriptProvider scriptProvider) => this.scriptProviders.Remove(scriptProvider);
    }
}