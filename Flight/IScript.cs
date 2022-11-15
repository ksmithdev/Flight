namespace Flight;

/// <summary>
/// Represents an interface for a script that gets applied against a database during a migration.
/// </summary>
public interface IScript
{
    /// <summary>
    /// Gets the script checksum used to detect changes.
    /// </summary>
    string Checksum { get; }

    /// <summary>
    /// Gets a value indicating whether the script is idempotent.
    /// </summary>
    bool Idempotent { get; }

    /// <summary>
    /// Gets the unique script name.
    /// </summary>
    string ScriptName { get; }

    /// <summary>
    /// Gets the script text.
    /// </summary>
    string Text { get; }
}