namespace Flight
{
    /// <summary>
    /// Defines an interface for a script that gets applied against a database during a migration
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// Get the script checksum used to detect changes.
        /// </summary>
        string Checksum { get; }

        /// <summary>
        /// Get whether the script is idempotent.
        /// </summary>
        bool Idempotent { get; }

        /// <summary>
        /// Get the unique script name.
        /// </summary>
        string ScriptName { get; }

        /// <summary>
        /// Get the script text
        /// </summary>
        string Text { get; }
    }
}