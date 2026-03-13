using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Domain.Tests.Models;

public class StackInfoTests
{
    [Fact]
    public void HasBack_ReturnsTrueWhenBackPathPresent()
    {
        var stack = new StackInfo
        {
            Id = "IMG_001",
            BackPath = "/photos/IMG_001_b.jpg",
        };

        Assert.True(stack.HasBack);
    }

    [Fact]
    public void HasBack_ReturnsFalseWhenNoBackPath()
    {
        var stack = new StackInfo { Id = "IMG_002" };
        Assert.False(stack.HasBack);
    }

    [Fact]
    public void DefaultEnrichmentStatus_IsPending()
    {
        var stack = new StackInfo { Id = "IMG_003" };
        Assert.Equal(EnrichmentStatus.Pending, stack.EnrichmentStatus);
    }
}
