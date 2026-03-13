namespace PhotostaxAiCurator.Domain.Interfaces;

using PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Abstraction over the photostax .NET library for dependency injection and testability.
/// </summary>
public interface IPhotostaxAdapter : IDisposable
{
    /// <summary>Open a repository at the given path.</summary>
    void Open(string directoryPath, bool recursive = false);

    /// <summary>Scan all photo stacks (lightweight, no metadata).</summary>
    List<StackInfo> Scan();

    /// <summary>Get a single stack with loaded metadata.</summary>
    StackInfo GetStackWithMetadata(string stackId);

    /// <summary>Read raw image bytes for a given file path.</summary>
    byte[] ReadImage(string path);

    /// <summary>Write AI-enriched metadata back to the stack.</summary>
    void WriteMetadata(string stackId, Dictionary<string, string> xmpTags, Dictionary<string, object?> customTags);

    /// <summary>Whether a repository is currently open.</summary>
    bool IsOpen { get; }
}
