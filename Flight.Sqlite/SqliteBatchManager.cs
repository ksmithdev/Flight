using Flight.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flight
{
    internal class SqliteBatchManager : IBatchManager
    {
        public IEnumerable<string> Split(IScript script) => new string[] { script.Text };
    }
}