using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Domain.Tests.Models;

public class BatchProgressTests
{
    [Fact]
    public void PercentComplete_CalculatesCorrectly()
    {
        var progress = new BatchProgress { TotalStacks = 100, Completed = 42 };
        Assert.Equal(42.0, progress.PercentComplete);
    }

    [Fact]
    public void PercentComplete_ReturnsZero_WhenNoStacks()
    {
        var progress = new BatchProgress { TotalStacks = 0, Completed = 0 };
        Assert.Equal(0.0, progress.PercentComplete);
    }
}
