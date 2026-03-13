namespace PhotostaxAiCurator.Domain.Interfaces;

using PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Manages the queue of items that need human review.
/// </summary>
public interface IReviewQueueService
{
    /// <summary>Enqueue an item for review.</summary>
    void Enqueue(ReviewItem item);

    /// <summary>Get the next item to review (lowest confidence first).</summary>
    ReviewItem? Dequeue();

    /// <summary>Get all items currently in the queue.</summary>
    IReadOnlyList<ReviewItem> GetAll();

    /// <summary>Number of items awaiting review.</summary>
    int Count { get; }

    /// <summary>Mark an item as approved and ready for write-back.</summary>
    void Approve(string stackId);

    /// <summary>Mark an item as skipped/rejected.</summary>
    void Skip(string stackId);
}
