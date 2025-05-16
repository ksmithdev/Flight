namespace Flight.Providers;

using System;
using System.IO;
using System.Reflection;

/// <summary>
/// Represents a script from the embedded resource provider.
/// </summary>
internal class EmbeddedResourceScript : ScriptBase
{
    private readonly string resourceName;
    private readonly Lazy<string> text;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddedResourceScript"/> class.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource.</param>
    /// <param name="idempotent">Whether the script is idempotent.</param>
    public EmbeddedResourceScript(string resourceName, bool idempotent)
    {
        this.resourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));

        this.text = new Lazy<string>(this.GetText);

        this.ScriptName = Path.GetFileName(resourceName);
        this.Idempotent = idempotent;
    }

    /// <inheritdoc/>
    public override bool Idempotent { get; }

    /// <inheritdoc/>
    public override string ScriptName { get; }

    /// <inheritdoc/>
    public override string Text => this.text.Value;

    private string GetText()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(this.resourceName) ?? throw new InvalidOperationException($"Embedded resource '{this.resourceName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}