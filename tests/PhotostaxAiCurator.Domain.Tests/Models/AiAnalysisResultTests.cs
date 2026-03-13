using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.Domain.Tests.Models;

public class AiAnalysisResultTests
{
    [Fact]
    public void DefaultResult_HasEmptyCollections()
    {
        var result = new AiAnalysisResult();

        Assert.Empty(result.People);
        Assert.Empty(result.Places);
        Assert.Empty(result.Events);
        Assert.Empty(result.Holidays);
        Assert.Empty(result.Objects);
        Assert.Empty(result.Colors);
        Assert.Empty(result.Landmarks);
        Assert.Empty(result.NeedsHumanReview);
        Assert.Equal(0, result.PeopleCount);
        Assert.Equal(0.0, result.OverallConfidence);
    }

    [Fact]
    public void Result_CanStoreComprehensiveMetadata()
    {
        var result = new AiAnalysisResult
        {
            Title = "Beach Day",
            Description = "Family at the beach",
            DateEstimate = "1985-07-04",
            DateSource = "ocr_back",
            DateConfidence = 0.92,
            Era = "1980s",
            People = [new PersonDetection { Name = "Alice", Description = "woman in hat", Confidence = 0.85 }],
            PeopleCount = 1,
            Places = ["Santa Cruz Beach"],
            Landmarks = [new LandmarkDetection { Name = "Santa Cruz Boardwalk", Confidence = 0.88 }],
            LocationEstimate = new LocationInfo { Latitude = 36.96, Longitude = -122.02, PlaceName = "Santa Cruz, CA", Confidence = 0.80 },
            Events = ["Family Vacation"],
            Holidays = [],
            Mood = "joyful",
            Scene = "outdoor beach",
            Objects = ["surfboard", "umbrella"],
            Colors = ["blue", "yellow"],
            OcrFront = null,
            OcrBack = "July 4th 1985 - Santa Cruz",
            HandwritingBack = "July 4th 1985 - Santa Cruz",
            OverallConfidence = 0.88,
            NeedsHumanReview = ["people.names"],
            ReviewReason = "Cannot confirm person identity from image alone",
        };

        Assert.Equal("Beach Day", result.Title);
        Assert.Single(result.People);
        Assert.Equal("Alice", result.People[0].Name);
        Assert.Equal("1985-07-04", result.DateEstimate);
        Assert.NotNull(result.LocationEstimate);
        Assert.Equal(36.96, result.LocationEstimate.Latitude);
    }
}
