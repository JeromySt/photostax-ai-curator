using System.Collections.Concurrent;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Services.Pipeline;

/// <summary>
/// In-memory review queue sorted by confidence (lowest first).
/// Thread-safe for concurrent batch processing.
/// </summary>
public sealed class ReviewQueueService : IReviewQueueService
{
    private readonly ConcurrentDictionary<string, ReviewItem> _items = new();

    public void Enqueue(ReviewItem item)
        => _items.TryAdd(item.StackId, item);

    public ReviewItem? Dequeue()
    {
        var next = _items.Values
            .Where(i => i.Status == EnrichmentStatus.NeedsReview)
            .OrderBy(i => i.AiResult.OverallConfidence)
            .FirstOrDefault();

        return next;
    }

    public IReadOnlyList<ReviewItem> GetAll()
        => _items.Values.OrderBy(i => i.AiResult.OverallConfidence).ToList();

    public int Count => _items.Values.Count(i => i.Status == EnrichmentStatus.NeedsReview);

    public void Approve(string stackId)
    {
        if (_items.TryGetValue(stackId, out var item))
        {
            item.Status = EnrichmentStatus.Approved;
            item.ReviewedAt = DateTime.UtcNow;
        }
    }

    public void Skip(string stackId)
    {
        if (_items.TryGetValue(stackId, out var item))
        {
            item.Status = EnrichmentStatus.Skipped;
            item.ReviewedAt = DateTime.UtcNow;
        }
    }
}
