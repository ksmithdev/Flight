namespace Flight.Providers
{
    using System.Collections.Generic;

    public abstract class ScriptProviderBase : IScriptProvider
    {
        protected ScriptProviderBase()
        {
        }

        public abstract IEnumerable<IScript> GetScripts();
    }
}