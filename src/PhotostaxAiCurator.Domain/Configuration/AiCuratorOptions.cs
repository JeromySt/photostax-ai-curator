namespace PhotostaxAiCurator.Domain.Configuration;

/// <summary>
/// Configuration options for the AI Curator application.
/// </summary>
public sealed class AiCuratorOptions
{
    public const string SectionName = "AiCurator";

    /// <summary>OpenAI API key.</summary>
    public string OpenAiApiKey { get; set; } = string.Empty;

    /// <summary>OpenAI model to use (default: gpt-4o).</summary>
    public string OpenAiModel { get; set; } = "gpt-4o";

    /// <summary>Confidence threshold for auto-approval (default: 0.85).</summary>
    public double AutoApproveThreshold { get; set; } = 0.85;

    /// <summary>Confidence threshold separating medium from low (default: 0.50).</summary>
    public double MediumConfidenceThreshold { get; set; } = 0.50;

    /// <summary>Maximum concurrent AI requests (default: 3).</summary>
    public int MaxConcurrency { get; set; } = 3;

    /// <summary>Maximum tokens for AI response (default: 4096).</summary>
    public int MaxResponseTokens { get; set; } = 4096;

    /// <summary>OpenAI API base URL (default: https://api.openai.com/v1).</summary>
    public string OpenAiBaseUrl { get; set; } = "https://api.openai.com/v1";
}
