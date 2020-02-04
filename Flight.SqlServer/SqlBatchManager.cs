using Flight.Database;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Flight
{
    internal class SqlBatchManager : IBatchManager
    {
        public IEnumerable<string> Split(IScript script)
        {
            // An iterator method (a method that contains `yield`) will not validate arguments until the caller begins to enumerate the result items.
            if (script == null)
                throw new ArgumentNullException(nameof(script));

            return ScriptIterator();

            IEnumerable<string> ScriptIterator()
            {
                foreach (var command in Regex.Split(script.Text, @"GO(?:\s+\d*)?(?:\r\n)?"))
                {
                    if (string.IsNullOrWhiteSpace(command))
                        continue;

                    yield return command;
                }
            }
        }
    }
}