using System.Collections.Generic;

namespace Flight.Providers
{
    public class CompositeScriptProvider : ScriptProviderBase
    {
        private readonly ISet<IScriptProvider> scriptProviders;

        public CompositeScriptProvider()
        {
            scriptProviders = new HashSet<IScriptProvider>();
        }

        public CompositeScriptProvider(params IScriptProvider[] scriptProviders)
        {
            this.scriptProviders = new HashSet<IScriptProvider>(scriptProviders);
        }

        public void AddScriptProvider(IScriptProvider scriptProvider) => scriptProviders.Add(scriptProvider);

        public override IEnumerable<IScript> GetScripts()
        {
            foreach (var scriptProvider in scriptProviders)
            {
                foreach (var script in scriptProvider.GetScripts())
                {
                    yield return script;
                }
            }
        }

        public bool RemoveScriptProvider(IScriptProvider scriptProvider) => scriptProviders.Remove(scriptProvider);
    }
}