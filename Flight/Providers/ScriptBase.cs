namespace Flight.Providers
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents an abstract script base class.
    /// </summary>
    public abstract class ScriptBase : IScript
    {
        private readonly Lazy<byte[]> bytes;
        private readonly Lazy<string> checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBase"/> class.
        /// </summary>
        protected ScriptBase()
        {
            this.bytes = new Lazy<byte[]>(this.GetBytes);
            this.checksum = new Lazy<string>(this.GetChecksum);
        }

        /// <inheritdoc/>
        public string Checksum => this.checksum.Value;

        /// <inheritdoc/>
        public abstract bool Idempotent { get; }

        /// <inheritdoc/>
        public abstract string ScriptName { get; }

        /// <inheritdoc/>
        public abstract string Text { get; }

        private byte[] GetBytes() => Encoding.UTF8.GetBytes(this.Text);

        private string GetChecksum()
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(this.bytes.Value);

            return Convert.ToBase64String(hash);
        }
    }
}