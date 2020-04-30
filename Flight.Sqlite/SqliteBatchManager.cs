namespace Flight
{
    using Flight.Database;
    using System.Collections.Generic;

    internal class SqliteBatchManager : IBatchManager
    {
        public IEnumerable<string> Split(IScript script) => new string[] { script.Text };
    }
}