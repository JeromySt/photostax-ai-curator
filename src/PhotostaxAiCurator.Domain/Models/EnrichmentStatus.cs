namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Tracks the enrichment state of a photo stack through the AI pipeline.
/// </summary>
public enum EnrichmentStatus
{
    /// <summary>Not yet analyzed by AI.</summary>
    Pending,

    /// <summary>Currently being analyzed.</summary>
    Analyzing,

    /// <summary>AI analysis complete, auto-approved (high confidence).</summary>
    AutoApproved,

    /// <summary>AI analysis complete, awaiting human review (medium/low confidence).</summary>
    NeedsReview,

    /// <summary>Human has reviewed and approved the AI suggestions.</summary>
    Approved,

    /// <summary>Metadata has been written back to photostax.</summary>
    Written,

    /// <summary>AI analysis failed (API error, timeout, etc.).</summary>
    Failed,

    /// <summary>User explicitly skipped or rejected this stack.</summary>
    Skipped
}
