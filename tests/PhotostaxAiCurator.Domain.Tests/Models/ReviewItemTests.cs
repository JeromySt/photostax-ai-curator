using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Domain.Tests.Models;

public class ReviewItemTests
{
    [Fact]
    public void NewReviewItem_HasCorrectDefaults()
    {
        var item = new ReviewItem
        {
            StackId = "IMG_001",
            AiResult = new AiAnalysisResult { Title = "Test" },
        };

        Assert.Equal("IMG_001", item.StackId);
        Assert.Equal(EnrichmentStatus.NeedsReview, item.Status);
        Assert.Equal(ConfidenceLevel.Unknown, item.Confidence);
        Assert.Null(item.ReviewedAt);
        Assert.Empty(item.UserOverrides);
    }

    [Fact]
    public void UserOverrides_CanBeApplied()
    {
        var item = new ReviewItem
        {
            StackId = "IMG_002",
            AiResult = new AiAnalysisResult { Title = "AI Title", Description = "AI Description" },
        };

        item.UserOverrides["title"] = "User Title";
        item.UserOverrides["date"] = "2020-01-01";

        Assert.Equal(2, item.UserOverrides.Count);
        Assert.Equal("User Title", item.UserOverrides["title"]);
    }
}
