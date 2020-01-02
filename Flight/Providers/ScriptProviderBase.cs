using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;

namespace Flight.Providers
{
    public abstract class ScriptProviderBase : IScriptProvider
    {
        protected ScriptProviderBase()
        {
        }

        protected ILogger Logger { get; private set; } = NullLogger.Instance;

        public abstract IEnumerable<IScript> GetScripts();

        public virtual void Initialize(ILoggerFactory loggerFactory) => Logger = loggerFactory.CreateLogger(GetType());
    }
}