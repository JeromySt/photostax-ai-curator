namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Geographic location estimate from AI analysis.
/// </summary>
public sealed class LocationInfo
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PlaceName { get; set; }
    public double Confidence { get; set; }
}
