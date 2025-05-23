﻿namespace Flight.Providers;

using System;
using System.IO;

/// <summary>
/// Represents a script from the file system provider.
/// </summary>
internal class FileSystemScript : ScriptBase
{
    private readonly string path;
    private readonly Lazy<string> text;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemScript"/> class.
    /// </summary>
    /// <param name="path">The path to the script file.</param>
    /// <param name="idempotent">Whether the script is idempotent.</param>
    public FileSystemScript(string path, bool idempotent)
    {
        this.path = path ?? throw new ArgumentNullException(nameof(path));

        text = new Lazy<string>(GetText);

        ScriptName = Path.GetFileName(path);
        Idempotent = idempotent;
    }

    /// <inheritdoc/>
    public override bool Idempotent { get; }

    /// <inheritdoc/>
    public override string ScriptName { get; }

    /// <inheritdoc/>
    public override string Text => text.Value;

    private string GetText() => File.ReadAllText(path);
}