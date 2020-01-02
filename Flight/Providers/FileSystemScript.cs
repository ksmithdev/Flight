using System;
using System.IO;
using System.Text;

namespace Flight.Providers
{
    internal class FileSystemScript : IScript
    {
        private readonly Lazy<byte[]> bytes;
        private readonly Lazy<string> checksum;
        private readonly string path;
        private readonly Lazy<string> text;

        public FileSystemScript(string path, bool idempotent)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));

            bytes = new Lazy<byte[]>(GetBytes);
            checksum = new Lazy<string>(GetChecksum);
            text = new Lazy<string>(GetText);

            ScriptName = Path.GetFileName(path);
            Idempotent = idempotent;
        }

        public string Checksum => checksum.Value;

        public bool Idempotent { get; }

        public string ScriptName { get; }

        public string Text => text.Value;

        private byte[] GetBytes() => Encoding.UTF8.GetBytes(Text);

        private string GetChecksum()
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(bytes.Value);

            return Convert.ToBase64String(hash);
        }

        private string GetText() => File.ReadAllText(path);
    }
}