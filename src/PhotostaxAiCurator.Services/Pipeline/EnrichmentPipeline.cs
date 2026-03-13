using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotostaxAiCurator.Domain.Configuration;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Services.Pipeline;

/// <summary>
/// Orchestrates: load stack → send to AI → evaluate confidence → route to auto-approve or review queue → write metadata.
/// </summary>
public sealed class EnrichmentPipeline : IEnrichmentPipeline
{
    private readonly IAiVisionService _aiVision;
    private readonly IPhotostaxAdapter _photostax;
    private readonly IReviewQueueService _reviewQueue;
    private readonly AiCuratorOptions _options;
    private readonly ILogger<EnrichmentPipeline> _logger;

    public EnrichmentPipeline(
        IAiVisionService aiVision,
        IPhotostaxAdapter photostax,
        IReviewQueueService reviewQueue,
        IOptions<AiCuratorOptions> options,
        ILogger<EnrichmentPipeline> logger)
    {
        _aiVision = aiVision;
        _photostax = photostax;
        _reviewQueue = reviewQueue;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<ReviewItem> AnalyzeStackAsync(string stackId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Analyzing stack {StackId}", stackId);

        var stack = _photostax.GetStackWithMetadata(stackId);
        var images = LoadImages(stack);

        var existingMetadata = stack.ExifTags
            .Concat(stack.XmpTags)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var aiResult = await _aiVision.AnalyzeAsync(images, existingMetadata, cancellationToken);

        var confidence = ConfidenceLevelExtensions.Categorize(
            aiResult.OverallConfidence,
            _options.AutoApproveThreshold,
            _options.MediumConfidenceThreshold);

        var status = confidence == ConfidenceLevel.High
            ? EnrichmentStatus.AutoApproved
            : EnrichmentStatus.NeedsReview;

        var reviewItem = new ReviewItem
        {
            StackId = stackId,
            AiResult = aiResult,
            Status = status,
            Confidence = confidence,
        };

        if (status == EnrichmentStatus.NeedsReview)
        {
            _reviewQueue.Enqueue(reviewItem);
            _logger.LogInformation("Stack {StackId} queued for review (confidence: {Confidence:P0})",
                stackId, aiResult.OverallConfidence);
        }
        else
        {
            _logger.LogInformation("Stack {StackId} auto-approved (confidence: {Confidence:P0})",
                stackId, aiResult.OverallConfidence);
        }

        return reviewItem;
    }

    public async Task RunBatchAsync(
        IProgress<BatchProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var stacks = _photostax.Scan();
        var batchProgress = new BatchProgress { TotalStacks = stacks.Count };

        _logger.LogInformation("Starting batch analysis of {Count} stacks", stacks.Count);

        using var semaphore = new SemaphoreSlim(_options.MaxConcurrency);

        var tasks = stacks.Select(async stack =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                batchProgress.CurrentStackId = stack.Id;
                progress?.Report(batchProgress);

                try
                {
                    var result = await AnalyzeStackAsync(stack.Id, cancellationToken);

                    lock (batchProgress)
                    {
                        batchProgress.Completed++;
                        if (result.Status == EnrichmentStatus.AutoApproved)
                            batchProgress.AutoApproved++;
                        else
                            batchProgress.NeedsReview++;
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogWarning(ex, "Failed to analyze stack {StackId}", stack.Id);
                    lock (batchProgress)
                    {
                        batchProgress.Completed++;
                        batchProgress.Failed++;
                    }
                }

                progress?.Report(batchProgress);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        _logger.LogInformation(
            "Batch complete: {Auto} auto-approved, {Review} need review, {Failed} failed",
            batchProgress.AutoApproved, batchProgress.NeedsReview, batchProgress.Failed);
    }

    public Task WriteMetadataAsync(ReviewItem item, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ai = item.AiResult;
        var xmpTags = new Dictionary<string, string>();
        var customTags = new Dictionary<string, object?>();

        // Standard XMP (Dublin Core) — readable by any photo viewer
        if (ai.Title is not null) xmpTags["title"] = ai.Title;
        if (ai.Description is not null) xmpTags["description"] = ai.Description;
        if (ai.DateEstimate is not null) xmpTags["date"] = ai.DateEstimate;

        // Build subject keywords from people, places, events, objects
        var keywords = new List<string>();
        keywords.AddRange(ai.People.Where(p => p.Name is not null).Select(p => p.Name!));
        keywords.AddRange(ai.Places);
        keywords.AddRange(ai.Events);
        keywords.AddRange(ai.Objects);
        if (keywords.Count > 0)
            xmpTags["subject"] = string.Join(", ", keywords.Distinct());

        // Custom tags (structured JSON in sidecar)
        if (ai.People.Count > 0)
            customTags["people"] = ai.People.Where(p => p.Name is not null).Select(p => p.Name).ToList();
        if (ai.Places.Count > 0) customTags["places"] = ai.Places;
        if (ai.Events.Count > 0) customTags["events"] = ai.Events;
        if (ai.Holidays.Count > 0) customTags["holidays"] = ai.Holidays;
        if (ai.LocationEstimate is not null)
            customTags["location"] = new { lat = ai.LocationEstimate.Latitude, lng = ai.LocationEstimate.Longitude };
        if (ai.Era is not null) customTags["era"] = ai.Era;
        if (ai.Mood is not null) customTags["mood"] = ai.Mood;
        if (ai.Scene is not null) customTags["scene"] = ai.Scene;
        if (ai.OcrFront is not null) customTags["ocr_front"] = ai.OcrFront;
        if (ai.OcrBack is not null) customTags["ocr_back"] = ai.OcrBack;
        if (ai.HandwritingBack is not null) customTags["handwriting_back"] = ai.HandwritingBack;
        if (ai.Caption is not null) customTags["caption"] = ai.Caption;
        if (ai.Objects.Count > 0) customTags["objects"] = ai.Objects;
        if (ai.Colors.Count > 0) customTags["colors"] = ai.Colors;
        customTags["confidence"] = ai.OverallConfidence;

        // Apply user overrides
        foreach (var (key, value) in item.UserOverrides)
        {
            if (xmpTags.ContainsKey(key) && value is string strVal)
                xmpTags[key] = strVal;
            else
                customTags[key] = value;
        }

        _photostax.WriteMetadata(item.StackId, xmpTags, customTags);
        item.Status = EnrichmentStatus.Written;

        _logger.LogInformation("Metadata written for stack {StackId}", item.StackId);

        return Task.CompletedTask;
    }

    private Dictionary<ImageSide, byte[]> LoadImages(StackInfo stack)
    {
        var images = new Dictionary<ImageSide, byte[]>();

        // Prefer enhanced over original for front analysis
        if (stack.EnhancedPath is not null)
            images[ImageSide.Enhanced] = _photostax.ReadImage(stack.EnhancedPath);
        else if (stack.OriginalPath is not null)
            images[ImageSide.Original] = _photostax.ReadImage(stack.OriginalPath);

        // Always include back if available (critical for OCR)
        if (stack.BackPath is not null)
            images[ImageSide.Back] = _photostax.ReadImage(stack.BackPath);

        return images;
    }
}
