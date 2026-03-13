using PhotostaxAiCurator.Services.AI;

namespace PhotostaxAiCurator.Services.Tests.AI;

public class OpenAiPromptsTests
{
    [Fact]
    public void SystemPrompt_IsNotEmpty()
    {
        Assert.NotEmpty(OpenAiPrompts.SystemPrompt);
        Assert.Contains("photo metadata analyst", OpenAiPrompts.SystemPrompt);
    }

    [Fact]
    public void UserPromptTemplate_ContainsPlaceholder()
    {
        Assert.Contains("{0}", OpenAiPrompts.UserPromptTemplate);
    }

    [Fact]
    public void GetContextNote_WithNullMetadata_ReturnsDefault()
    {
        var note = OpenAiPrompts.GetContextNote(null);
        Assert.Contains("No existing metadata", note);
    }

    [Fact]
    public void GetContextNote_WithEmptyMetadata_ReturnsDefault()
    {
        var note = OpenAiPrompts.GetContextNote(new Dictionary<string, string>());
        Assert.Contains("No existing metadata", note);
    }

    [Fact]
    public void GetContextNote_WithMetadata_FormatsCorrectly()
    {
        var metadata = new Dictionary<string, string>
        {
            ["Make"] = "EPSON",
            ["DateTime"] = "2024:01:15",
        };

        var note = OpenAiPrompts.GetContextNote(metadata);

        Assert.Contains("Make: EPSON", note);
        Assert.Contains("DateTime: 2024:01:15", note);
    }
}
