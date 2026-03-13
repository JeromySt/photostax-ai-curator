using System.Text.Json;
using PhotostaxAiCurator.Services.AI;

namespace PhotostaxAiCurator.Services.Tests.AI;

public class AiResponseParserTests
{
    [Fact]
    public void Parse_ValidJson_ReturnsResult()
    {
        var json = """
        {
            "title": "Beach Day",
            "description": "A family enjoying the beach",
            "date_estimate": "1985-07-04",
            "date_source": "ocr_back",
            "date_confidence": 0.92,
            "era": "1980s",
            "people": [
                {"name": "Alice", "description": "woman in hat", "face_position": "left", "confidence": 0.85}
            ],
            "people_count": 1,
            "places": ["Santa Cruz"],
            "landmarks": [{"name": "Boardwalk", "confidence": 0.88}],
            "location_estimate": {"latitude": 36.96, "longitude": -122.02, "place_name": "Santa Cruz, CA", "confidence": 0.80},
            "events": ["vacation"],
            "holidays": [],
            "mood": "joyful",
            "scene": "outdoor beach",
            "objects": ["surfboard"],
            "colors": ["blue"],
            "ocr_front": null,
            "ocr_back": "July 4th 1985",
            "handwriting_back": "July 4th 1985",
            "overall_confidence": 0.88,
            "needs_human_review": ["people.names"],
            "review_reason": "Cannot confirm identity"
        }
        """;

        var result = AiResponseParser.Parse(json);

        Assert.Equal("Beach Day", result.Title);
        Assert.Equal("1985-07-04", result.DateEstimate);
        Assert.Equal(0.92, result.DateConfidence);
        Assert.Single(result.People);
        Assert.Equal("Alice", result.People[0].Name);
        Assert.Equal("joyful", result.Mood);
        Assert.Equal(0.88, result.OverallConfidence);
        Assert.Equal("July 4th 1985", result.OcrBack);
        Assert.NotNull(result.LocationEstimate);
    }

    [Fact]
    public void Parse_WithMarkdownFences_StripsAndParses()
    {
        var json = """
        ```json
        {"title": "Test Photo", "overall_confidence": 0.75, "people": [], "places": [], "events": [], "holidays": [], "objects": [], "colors": [], "landmarks": [], "needs_human_review": []}
        ```
        """;

        var result = AiResponseParser.Parse(json);
        Assert.Equal("Test Photo", result.Title);
        Assert.Equal(0.75, result.OverallConfidence);
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => AiResponseParser.Parse("not valid json"));
    }

    [Fact]
    public void Parse_MinimalJson_ReturnsWithDefaults()
    {
        var json = """{"title": "Minimal", "people": [], "places": [], "events": [], "holidays": [], "objects": [], "colors": [], "landmarks": [], "needs_human_review": []}""";

        var result = AiResponseParser.Parse(json);

        Assert.Equal("Minimal", result.Title);
        Assert.Null(result.Description);
        Assert.Null(result.DateEstimate);
        Assert.Empty(result.People);
        Assert.Equal(0.0, result.OverallConfidence);
    }

    [Fact]
    public void ToJson_RoundTrips()
    {
        var original = AiResponseParser.Parse("""
        {"title": "Roundtrip", "description": "Test", "overall_confidence": 0.9, "people": [], "places": ["Paris"], "events": [], "holidays": [], "objects": [], "colors": [], "landmarks": [], "needs_human_review": []}
        """);

        var json = AiResponseParser.ToJson(original);
        var parsed = AiResponseParser.Parse(json);

        Assert.Equal(original.Title, parsed.Title);
        Assert.Equal(original.OverallConfidence, parsed.OverallConfidence);
        Assert.Equal(original.Places, parsed.Places);
    }
}
