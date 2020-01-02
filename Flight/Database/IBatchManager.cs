using System.Collections.Generic;

namespace Flight.Database
{
    public interface IBatchManager
    {
        string Combine(IEnumerable<IScript> commands);

        IEnumerable<string> Split(IScript script);
    }
}