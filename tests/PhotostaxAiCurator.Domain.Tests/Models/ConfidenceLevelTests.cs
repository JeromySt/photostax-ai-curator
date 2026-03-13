using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Domain.Tests.Models;

public class ConfidenceLevelTests
{
    [Theory]
    [InlineData(0.90, ConfidenceLevel.High)]
    [InlineData(0.85, ConfidenceLevel.High)]
    [InlineData(0.84, ConfidenceLevel.Medium)]
    [InlineData(0.50, ConfidenceLevel.Medium)]
    [InlineData(0.49, ConfidenceLevel.Low)]
    [InlineData(0.01, ConfidenceLevel.Low)]
    [InlineData(0.0, ConfidenceLevel.Unknown)]
    public void Categorize_WithDefaultThresholds_ReturnsExpected(double confidence, ConfidenceLevel expected)
    {
        var result = ConfidenceLevelExtensions.Categorize(confidence);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Categorize_WithCustomThresholds_ReturnsExpected()
    {
        Assert.Equal(ConfidenceLevel.High, ConfidenceLevelExtensions.Categorize(0.95, highThreshold: 0.90));
        Assert.Equal(ConfidenceLevel.Medium, ConfidenceLevelExtensions.Categorize(0.89, highThreshold: 0.90));
        Assert.Equal(ConfidenceLevel.Low, ConfidenceLevelExtensions.Categorize(0.20, highThreshold: 0.90, mediumThreshold: 0.30));
    }
}
