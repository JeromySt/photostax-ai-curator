namespace PhotostaxAiCurator.Domain.Interfaces;

using PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Orchestrates the full enrichment pipeline: scan → analyze → classify → review → write.
/// </summary>
public interface IEnrichmentPipeline
{
    /// <summary>
    /// Analyze a single photo stack and return the review item.
    /// </summary>
    Task<ReviewItem> AnalyzeStackAsync(string stackId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Run batch analysis on all (or filtered) stacks with progress reporting.
    /// </summary>
    Task RunBatchAsync(
        IProgress<BatchProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Write approved metadata back to the photostax repository.
    /// </summary>
    Task WriteMetadataAsync(ReviewItem item, CancellationToken cancellationToken = default);
}
