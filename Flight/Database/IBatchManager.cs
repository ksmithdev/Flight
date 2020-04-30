namespace Flight.Database
{
    using System.Collections.Generic;

    public interface IBatchManager
    {
        IEnumerable<string> Split(IScript script);
    }
}