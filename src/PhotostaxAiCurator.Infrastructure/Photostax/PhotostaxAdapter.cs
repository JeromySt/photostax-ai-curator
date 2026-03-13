using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Infrastructure.Photostax;

/// <summary>
/// Wraps the Photostax .NET binding (P/Invoke to photostax-ffi) for DI and testability.
/// When the Photostax NuGet package is available, this adapter maps between
/// Photostax types and our domain types.
/// </summary>
public sealed class PhotostaxAdapter : IPhotostaxAdapter
{
    private global::Photostax.PhotostaxRepository? _repo;

    public bool IsOpen => _repo is not null;

    public void Open(string directoryPath, bool recursive = false)
    {
        _repo?.Dispose();
        _repo = new global::Photostax.PhotostaxRepository(directoryPath, recursive);
    }

    public List<StackInfo> Scan()
    {
        EnsureOpen();
        var stacks = _repo!.Scan();
        return stacks.Select(s => MapStack(s)).ToList();
    }

    public StackInfo GetStackWithMetadata(string stackId)
    {
        EnsureOpen();
        var stack = _repo!.GetStack(stackId);
        var metadata = _repo.LoadMetadata(stackId);
        return MapStack(stack, metadata);
    }

    public byte[] ReadImage(string path)
    {
        EnsureOpen();
        return _repo!.ReadImage(path);
    }

    public void WriteMetadata(string stackId, Dictionary<string, string> xmpTags, Dictionary<string, object?> customTags)
    {
        EnsureOpen();

        // Build a Metadata object using the WithCustomTag builder
        var metadata = new global::Photostax.Metadata();

        // XMP tags (title, description, subject, date, etc.) — written to Dublin Core namespace
        foreach (var (key, value) in xmpTags)
            metadata = metadata.WithCustomTag(key, value);

        // Custom tags (structured JSON in sidecar: people, events, location, etc.)
        foreach (var (key, value) in customTags)
            metadata = metadata.WithCustomTag(key, value);

        _repo!.WriteMetadata(stackId, metadata);
    }

    public void Dispose()
    {
        _repo?.Dispose();
        _repo = null;
    }

    private void EnsureOpen()
    {
        if (_repo is null)
            throw new InvalidOperationException("No repository is open. Call Open() first.");
    }

    private static StackInfo MapStack(global::Photostax.PhotoStack stack, global::Photostax.Metadata? metadata = null)
    {
        return new StackInfo
        {
            Id = stack.Id,
            OriginalPath = stack.OriginalPath,
            EnhancedPath = stack.EnhancedPath,
            BackPath = stack.BackPath,
            ExifTags = metadata?.ExifTags?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? [],
            XmpTags = metadata?.XmpTags?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? [],
            CustomTags = metadata?.CustomTags?.ToDictionary(kv => kv.Key, kv => (object?)kv.Value) ?? [],
        };
    }
}
