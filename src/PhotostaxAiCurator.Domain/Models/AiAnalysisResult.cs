using System.Text.Json.Serialization;

namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Complete AI analysis result for a single photo stack.
/// Produced by sending front/back/enhanced images to GPT-4o Vision.
/// </summary>
public sealed class AiAnalysisResult
{
    // --- Descriptive ---
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Caption { get; set; }

    // --- Temporal ---
    public string? DateEstimate { get; set; }
    public string? DateSource { get; set; }
    public double DateConfidence { get; set; }
    public string? Era { get; set; }

    // --- People ---
    public List<PersonDetection> People { get; set; } = [];
    public int PeopleCount { get; set; }

    // --- Places & Location ---
    public List<string> Places { get; set; } = [];
    public List<LandmarkDetection> Landmarks { get; set; } = [];
    public LocationInfo? LocationEstimate { get; set; }

    // --- Events ---
    public List<string> Events { get; set; } = [];
    public List<string> Holidays { get; set; } = [];

    // --- Scene ---
    public string? Mood { get; set; }
    public string? Scene { get; set; }
    public List<string> Objects { get; set; } = [];
    public List<string> Colors { get; set; } = [];

    // --- OCR ---
    public string? OcrFront { get; set; }
    public string? OcrBack { get; set; }
    public string? HandwritingBack { get; set; }

    // --- Confidence & Review ---
    public double OverallConfidence { get; set; }
    public List<string> NeedsHumanReview { get; set; } = [];
    public string? ReviewReason { get; set; }
}
