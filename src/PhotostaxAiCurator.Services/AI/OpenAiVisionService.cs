using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotostaxAiCurator.Domain.Configuration;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Services.AI;

/// <summary>
/// Implements IAiVisionService using OpenAI's GPT-4o Vision API.
/// Sends front + back images in a single multimodal request.
/// </summary>
public sealed class OpenAiVisionService : IAiVisionService
{
    private readonly HttpClient _httpClient;
    private readonly AiCuratorOptions _options;
    private readonly ILogger<OpenAiVisionService> _logger;

    public OpenAiVisionService(
        HttpClient httpClient,
        IOptions<AiCuratorOptions> options,
        ILogger<OpenAiVisionService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiAnalysisResult> AnalyzeAsync(
        Dictionary<ImageSide, byte[]> images,
        Dictionary<string, string>? existingMetadata = null,
        CancellationToken cancellationToken = default)
    {
        var messages = BuildMessages(images, existingMetadata);
        var request = new OpenAiChatRequest
        {
            Model = _options.OpenAiModel,
            MaxTokens = _options.MaxResponseTokens,
            Messages = messages,
        };

        _logger.LogInformation("Sending {ImageCount} images to {Model} for analysis",
            images.Count, _options.OpenAiModel);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post,
            $"{_options.OpenAiBaseUrl}/chat/completions");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.OpenAiApiKey);
        httpRequest.Content = JsonContent.Create(request, options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        var chatResponse = JsonSerializer.Deserialize<OpenAiChatResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });

        var content = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content
            ?? throw new InvalidOperationException("No content in OpenAI response.");

        _logger.LogDebug("Received AI response ({Length} chars), parsing...", content.Length);

        return AiResponseParser.Parse(content);
    }

    private static List<OpenAiMessage> BuildMessages(
        Dictionary<ImageSide, byte[]> images,
        Dictionary<string, string>? existingMetadata)
    {
        var contentParts = new List<OpenAiContentPart>();

        // Text instructions
        var contextNote = OpenAiPrompts.GetContextNote(existingMetadata);
        contentParts.Add(new OpenAiContentPart
        {
            Type = "text",
            Text = string.Format(OpenAiPrompts.UserPromptTemplate, contextNote),
        });

        // Image parts
        foreach (var (side, bytes) in images.OrderBy(kv => kv.Key))
        {
            var label = side switch
            {
                ImageSide.Original => "FRONT of photo (original scan)",
                ImageSide.Enhanced => "FRONT of photo (enhanced/color-corrected)",
                ImageSide.Back => "BACK of photo (may have handwriting, dates, stamps)",
                _ => "Photo image",
            };

            contentParts.Add(new OpenAiContentPart
            {
                Type = "text",
                Text = $"[{label}]",
            });

            var base64 = Convert.ToBase64String(bytes);
            contentParts.Add(new OpenAiContentPart
            {
                Type = "image_url",
                ImageUrl = new OpenAiImageUrl
                {
                    Url = $"data:image/jpeg;base64,{base64}",
                    Detail = "high",
                },
            });
        }

        return
        [
            new OpenAiMessage { Role = "system", Content = OpenAiPrompts.SystemPrompt },
            new OpenAiMessage { Role = "user", ContentParts = contentParts },
        ];
    }

    // --- Internal DTOs for OpenAI API ---

    private sealed class OpenAiChatRequest
    {
        public string Model { get; set; } = "gpt-4o";

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; } = 4096;

        public List<OpenAiMessage> Messages { get; set; } = [];
    }

    private sealed class OpenAiMessage
    {
        public string Role { get; set; } = "user";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Content { get; set; }

        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OpenAiContentPart>? ContentParts { get; set; }
    }

    private sealed class OpenAiContentPart
    {
        public string Type { get; set; } = "text";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonPropertyName("image_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OpenAiImageUrl? ImageUrl { get; set; }
    }

    private sealed class OpenAiImageUrl
    {
        public string Url { get; set; } = string.Empty;
        public string Detail { get; set; } = "high";
    }

    private sealed class OpenAiChatResponse
    {
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private sealed class OpenAiChoice
    {
        public OpenAiResponseMessage? Message { get; set; }
    }

    private sealed class OpenAiResponseMessage
    {
        public string? Content { get; set; }
    }
}
