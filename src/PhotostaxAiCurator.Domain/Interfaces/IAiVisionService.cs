namespace PhotostaxAiCurator.Domain.Interfaces;

using PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Analyzes photo images using AI vision models.
/// Implementations may use OpenAI, Azure AI, or local models.
/// </summary>
public interface IAiVisionService
{
    /// <summary>
    /// Analyze one or more images from a photo stack and return comprehensive metadata.
    /// </summary>
    /// <param name="images">Dictionary of image side → raw image bytes.</param>
    /// <param name="existingMetadata">Any metadata already known (EXIF, folder name, etc.).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Structured analysis result with confidence scores.</returns>
    Task<AiAnalysisResult> AnalyzeAsync(
        Dictionary<ImageSide, byte[]> images,
        Dictionary<string, string>? existingMetadata = null,
        CancellationToken cancellationToken = default);
}
