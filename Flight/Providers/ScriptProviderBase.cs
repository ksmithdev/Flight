namespace Flight.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the abstract script provider class.
    /// </summary>
    public abstract class ScriptProviderBase : IScriptProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptProviderBase"/> class.
        /// </summary>
        protected ScriptProviderBase()
        {
        }

        /// <inheritdoc/>
        public abstract IEnumerable<IScript> GetScripts();
    }
}