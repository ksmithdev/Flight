using System.Collections.Generic;

namespace Flight.Providers
{
    public abstract class ScriptProviderBase : IScriptProvider
    {
        protected ScriptProviderBase()
        {
        }

        public abstract IEnumerable<IScript> GetScripts();
    }
}