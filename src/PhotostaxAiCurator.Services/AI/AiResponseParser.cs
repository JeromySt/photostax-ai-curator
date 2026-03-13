using System.Text.Json;
using System.Text.Json.Serialization;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Services.AI;

/// <summary>
/// Parses GPT-4o Vision JSON responses into strongly-typed AiAnalysisResult.
/// </summary>
public static class AiResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Parse raw JSON string from GPT-4o into an AiAnalysisResult.
    /// Handles common issues like markdown code fences in the response.
    /// </summary>
    public static AiAnalysisResult Parse(string json)
    {
        // Strip markdown code fences if present
        var cleaned = json.Trim();
        if (cleaned.StartsWith("```"))
        {
            var firstNewline = cleaned.IndexOf('\n');
            if (firstNewline >= 0)
                cleaned = cleaned[(firstNewline + 1)..];
            if (cleaned.EndsWith("```"))
                cleaned = cleaned[..^3];
            cleaned = cleaned.Trim();
        }

        return JsonSerializer.Deserialize<AiAnalysisResult>(cleaned, JsonOptions)
            ?? throw new JsonException("Failed to deserialize AI response into AiAnalysisResult.");
    }

    /// <summary>
    /// Serialize an AiAnalysisResult back to JSON (for debugging/logging).
    /// </summary>
    public static string ToJson(AiAnalysisResult result)
        => JsonSerializer.Serialize(result, JsonOptions);
}
