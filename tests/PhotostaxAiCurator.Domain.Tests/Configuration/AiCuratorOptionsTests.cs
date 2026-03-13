using PhotostaxAiCurator.Domain.Configuration;

namespace PhotostaxAiCurator.Domain.Tests.Configuration;

public class AiCuratorOptionsTests
{
    [Fact]
    public void Defaults_AreReasonable()
    {
        var options = new AiCuratorOptions();

        Assert.Equal("gpt-4o", options.OpenAiModel);
        Assert.Equal(0.85, options.AutoApproveThreshold);
        Assert.Equal(0.50, options.MediumConfidenceThreshold);
        Assert.Equal(3, options.MaxConcurrency);
        Assert.Equal(4096, options.MaxResponseTokens);
        Assert.Equal("https://api.openai.com/v1", options.OpenAiBaseUrl);
        Assert.Equal(string.Empty, options.OpenAiApiKey);
    }
}
