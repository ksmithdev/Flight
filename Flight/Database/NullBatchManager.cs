namespace Flight.Database;

using System.Collections.Generic;

/// <summary>
/// Reoresents the default batch manager that doesn't split the source scripts.
/// </summary>
internal class NullBatchManager : IBatchManager
{
    /// <inheritdoc/>
    public IEnumerable<string> Split(IScript script)
    {
        return new string[] { script.Text };
    }
}