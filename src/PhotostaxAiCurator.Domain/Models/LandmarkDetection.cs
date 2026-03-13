namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// A landmark or notable location identified in a photo.
/// </summary>
public sealed class LandmarkDetection
{
    public string Name { get; set; } = string.Empty;
    public double Confidence { get; set; }
}
