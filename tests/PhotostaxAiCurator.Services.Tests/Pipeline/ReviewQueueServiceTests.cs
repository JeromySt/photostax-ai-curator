using PhotostaxAiCurator.Domain.Models;
using PhotostaxAiCurator.Services.Pipeline;

namespace PhotostaxAiCurator.Services.Tests.Pipeline;

public class ReviewQueueServiceTests
{
    private readonly ReviewQueueService _queue = new();

    private static ReviewItem CreateItem(string id, double confidence) => new()
    {
        StackId = id,
        AiResult = new AiAnalysisResult { OverallConfidence = confidence },
        Status = EnrichmentStatus.NeedsReview,
        Confidence = ConfidenceLevelExtensions.Categorize(confidence),
    };

    [Fact]
    public void Enqueue_AddsItem()
    {
        _queue.Enqueue(CreateItem("IMG_001", 0.5));
        Assert.Equal(1, _queue.Count);
    }

    [Fact]
    public void Dequeue_ReturnsLowestConfidenceFirst()
    {
        _queue.Enqueue(CreateItem("HIGH", 0.80));
        _queue.Enqueue(CreateItem("LOW", 0.30));
        _queue.Enqueue(CreateItem("MED", 0.60));

        var next = _queue.Dequeue();
        Assert.NotNull(next);
        Assert.Equal("LOW", next.StackId);
    }

    [Fact]
    public void Dequeue_ReturnsNull_WhenEmpty()
    {
        Assert.Null(_queue.Dequeue());
    }

    [Fact]
    public void Count_OnlyCountsNeedsReview()
    {
        _queue.Enqueue(CreateItem("A", 0.5));
        _queue.Enqueue(CreateItem("B", 0.6));

        Assert.Equal(2, _queue.Count);

        _queue.Approve("A");
        Assert.Equal(1, _queue.Count);
    }

    [Fact]
    public void Approve_SetsStatusAndTimestamp()
    {
        var item = CreateItem("IMG_001", 0.5);
        _queue.Enqueue(item);

        _queue.Approve("IMG_001");

        var all = _queue.GetAll();
        var approved = all.Single(i => i.StackId == "IMG_001");
        Assert.Equal(EnrichmentStatus.Approved, approved.Status);
        Assert.NotNull(approved.ReviewedAt);
    }

    [Fact]
    public void Skip_SetsStatusAndTimestamp()
    {
        _queue.Enqueue(CreateItem("IMG_001", 0.5));

        _queue.Skip("IMG_001");

        var all = _queue.GetAll();
        var skipped = all.Single(i => i.StackId == "IMG_001");
        Assert.Equal(EnrichmentStatus.Skipped, skipped.Status);
        Assert.NotNull(skipped.ReviewedAt);
    }

    [Fact]
    public void GetAll_ReturnsSortedByConfidence()
    {
        _queue.Enqueue(CreateItem("C", 0.80));
        _queue.Enqueue(CreateItem("A", 0.30));
        _queue.Enqueue(CreateItem("B", 0.60));

        var all = _queue.GetAll();
        Assert.Equal(3, all.Count);
        Assert.Equal("A", all[0].StackId);
        Assert.Equal("B", all[1].StackId);
        Assert.Equal("C", all[2].StackId);
    }
}
