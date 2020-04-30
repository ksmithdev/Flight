namespace Flight.Providers
{
    using System.Collections.Generic;
    using System.Linq;

    public sealed class NullScriptProvider : IScriptProvider
    {
        public static readonly IScriptProvider Instance = new NullScriptProvider();
        
        public IEnumerable<IScript> GetScripts() => Enumerable.Empty<IScript>();
    }
}