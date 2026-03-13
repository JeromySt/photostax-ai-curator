namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Categorized confidence level for routing decisions.
/// </summary>
public enum ConfidenceLevel
{
    High,
    Medium,
    Low,
    Unknown
}

public static class ConfidenceLevelExtensions
{
    /// <summary>Default threshold: ≥0.85 = High, ≥0.50 = Medium, else Low.</summary>
    public static ConfidenceLevel Categorize(double confidence,
        double highThreshold = 0.85, double mediumThreshold = 0.50)
    {
        if (confidence >= highThreshold) return ConfidenceLevel.High;
        if (confidence >= mediumThreshold) return ConfidenceLevel.Medium;
        return confidence > 0 ? ConfidenceLevel.Low : ConfidenceLevel.Unknown;
    }
}
