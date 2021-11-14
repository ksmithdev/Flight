namespace Flight.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a script from an embedded assembly.
    /// </summary>
    public class EmbeddedScript : ScriptBase
    {
        private readonly string scriptName;
        private readonly bool idempodent;
        private readonly Lazy<string> text;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedScript"/> class.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="stream">The embeddeded source stream.</param>
        /// <param name="idempodent">Whetherthe script is idempotent.</param>
        public EmbeddedScript(string scriptName, Stream stream, bool idempodent)
        {
            this.scriptName = scriptName ?? throw new ArgumentNullException(nameof(scriptName));
            this.idempodent = idempodent;

            this.text = new Lazy<string>(() =>
            {
                using var reader = new StreamReader(stream);

                string text = reader.ReadToEnd();

                stream?.Dispose();

                return text;
            });
        }

        /// <inheritdoc/>
        public override bool Idempotent => this.idempodent;

        /// <inheritdoc/>
        public override string ScriptName => this.scriptName;

        /// <inheritdoc/>
        public override string Text => this.text.Value;
    }
}
