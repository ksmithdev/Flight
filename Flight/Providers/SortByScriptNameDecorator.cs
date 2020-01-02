using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Flight.Providers
{
    public class SortByScriptNameDecorator : IScriptProvider
    {
        private readonly IScriptProvider scriptProvider;

        public SortByScriptNameDecorator(IScriptProvider scriptProvider)
        {
            this.scriptProvider = scriptProvider;
        }

        public IEnumerable<IScript> GetScripts() => scriptProvider.GetScripts().OrderBy(s => s.ScriptName);

        public void Initialize(ILoggerFactory loggerFactory) => scriptProvider.Initialize(loggerFactory);
    }
}