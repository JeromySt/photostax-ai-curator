namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// A photo stack paired with its AI analysis, awaiting or completed human review.
/// </summary>
public sealed class ReviewItem
{
    public required string StackId { get; init; }
    public required AiAnalysisResult AiResult { get; init; }
    public EnrichmentStatus Status { get; set; } = EnrichmentStatus.NeedsReview;
    public ConfidenceLevel Confidence { get; set; } = ConfidenceLevel.Unknown;
    public DateTime AnalyzedAt { get; init; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    /// <summary>User-edited overrides (field name → corrected value).</summary>
    public Dictionary<string, object?> UserOverrides { get; set; } = [];
}
