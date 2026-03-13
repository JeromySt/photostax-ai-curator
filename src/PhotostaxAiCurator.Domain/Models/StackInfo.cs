namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// UI-friendly representation of a photostax PhotoStack.
/// Decouples the domain from the photostax .NET binding types.
/// </summary>
public sealed class StackInfo
{
    public required string Id { get; init; }
    public string? OriginalPath { get; init; }
    public string? EnhancedPath { get; init; }
    public string? BackPath { get; init; }
    public bool HasBack => BackPath is not null;
    public bool HasEnhanced => EnhancedPath is not null;

    /// <summary>Existing metadata from photostax (EXIF + XMP + custom merged).</summary>
    public Dictionary<string, string> ExifTags { get; init; } = [];
    public Dictionary<string, string> XmpTags { get; init; } = [];
    public Dictionary<string, object?> CustomTags { get; init; } = [];

    /// <summary>Enrichment state in the AI pipeline.</summary>
    public EnrichmentStatus EnrichmentStatus { get; set; } = EnrichmentStatus.Pending;
}
