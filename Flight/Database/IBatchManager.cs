using System.Collections.Generic;

namespace Flight.Database
{
    public interface IBatchManager
    {
        IEnumerable<string> Split(IScript script);
    }
}