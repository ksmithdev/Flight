namespace Flight.Database
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a batch manager used to split scripts into commands.
    /// </summary>
    /// <remarks>An example would be the GO statement used in SQL Server.</remarks>
    public interface IBatchManager
    {
        /// <summary>
        /// Return the collection of command texts from the supplied script.
        /// </summary>
        /// <param name="script">The script to split.</param>
        /// <returns>The collection of subscripts from the source script.</returns>
        IEnumerable<string> Split(IScript script);
    }
}