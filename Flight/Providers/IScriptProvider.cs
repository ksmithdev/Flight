using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Flight.Providers
{
    public interface IScriptProvider
    {
        IEnumerable<IScript> GetScripts();

        void Initialize(ILoggerFactory loggerFactory);
    }
}