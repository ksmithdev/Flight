namespace Flight.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a provider to load scripts into the migration plan.
    /// </summary>
    public interface IScriptProvider
    {
        /// <summary>
        /// Return a collection of scripts.
        /// </summary>
        /// <returns>The collection of scripts.</returns>
        IEnumerable<IScript> GetScripts();
    }
}