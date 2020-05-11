namespace Flight.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines a provider to load scripts into the migration plan.
    /// </summary>
    public interface IScriptProvider
    {
        /// <summary>
        /// Return a collection of scripts.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IScript> GetScripts();
    }
}