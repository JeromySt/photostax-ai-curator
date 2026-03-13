namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// A person detected in a photo by AI analysis.
/// </summary>
public sealed class PersonDetection
{
    /// <summary>User-assigned name, or null if unidentified.</summary>
    public string? Name { get; set; }

    /// <summary>AI-generated description (e.g., "woman in red dress, ~30s").</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Approximate position in image (e.g., "left", "center", "right").</summary>
    public string? FacePosition { get; set; }

    /// <summary>AI confidence in detection (0.0–1.0).</summary>
    public double Confidence { get; set; }
}
