using System;
using System.Text;

namespace Flight.Providers
{
    public abstract class ScriptBase : IScript
    {
        private readonly Lazy<byte[]> bytes;
        private readonly Lazy<string> checksum;

        protected ScriptBase()
        {
            bytes = new Lazy<byte[]>(GetBytes);
            checksum = new Lazy<string>(GetChecksum);
        }

        public string Checksum => checksum.Value;

        public abstract bool Idempotent { get; }

        public abstract string ScriptName { get; }

        public abstract string Text { get; }

        private byte[] GetBytes() => Encoding.UTF8.GetBytes(Text);

        private string GetChecksum()
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var hash = md5.ComputeHash(bytes.Value);

            return Convert.ToBase64String(hash);
        }
    }
}