namespace Flight.Providers
{
    using System.Collections.Generic;
    using Flight.Logging;

    /// <summary>
    /// Defines a composite script provider to group a collection of script providers into a single object.
    /// </summary>
    public class CompositeScriptProvider : ScriptProviderBase
    {
        private readonly ICollection<IScriptProvider> scriptProviders;

        /// <summary>
        /// Creates a new instance of <see cref="CompositeScriptProvider"/>.
        /// </summary>
        public CompositeScriptProvider()
        {
            scriptProviders = new List<IScriptProvider>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="CompositeScriptProvider"/> with the supplied list of providers.
        /// </summary>
        public CompositeScriptProvider(params IScriptProvider[] scriptProviders)
        {
            this.scriptProviders = new List<IScriptProvider>(scriptProviders);
        }

        /// <summary>
        /// Add the supplied provider to the composite collection.
        /// </summary>
        /// <param name="scriptProvider"></param>
        public void AddScriptProvider(IScriptProvider scriptProvider) => scriptProviders.Add(scriptProvider);

        /// <summary>
        /// <inheritdoc cref="IScriptProvider.GetScripts"/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IScript> GetScripts()
        {
            try
            {
                Log.Trace($"Begin {nameof(CompositeScriptProvider)}.{nameof(GetScripts)}");

                foreach (var scriptProvider in scriptProviders)
                {
                    foreach (var script in scriptProvider.GetScripts())
                    {
                        yield return script;
                    }
                }
            }
            finally
            {
                Log.Trace($"End {nameof(CompositeScriptProvider)}.{nameof(GetScripts)}");
            }
        }

        /// <summary>
        /// Remove the supplied provier from the composite collection.
        /// </summary>
        /// <param name="scriptProvider"></param>
        /// <returns></returns>
        public bool RemoveScriptProvider(IScriptProvider scriptProvider) => scriptProviders.Remove(scriptProvider);
    }
}