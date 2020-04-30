namespace Flight.Providers
{
    using System;
    using System.IO;

    internal class FileSystemScript : ScriptBase
    {
        private readonly string path;
        private readonly Lazy<string> text;

        public FileSystemScript(string path, bool idempotent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));

            text = new Lazy<string>(GetText);

            ScriptName = Path.GetFileName(path);
            Idempotent = idempotent;
        }

        public override bool Idempotent { get; }

        public override string ScriptName { get; }

        public override string Text => text.Value;

        private string GetText() => File.ReadAllText(path);
    }
}