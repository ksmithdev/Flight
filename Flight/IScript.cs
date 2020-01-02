namespace Flight
{
    public interface IScript
    {
        string Checksum { get; }

        bool Idempotent { get; }

        string ScriptName { get; }

        string Text { get; }
    }
}