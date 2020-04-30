namespace Flight.Providers
{
    using System.Collections.Generic;

    public interface IScriptProvider
    {
        IEnumerable<IScript> GetScripts();
    }
}