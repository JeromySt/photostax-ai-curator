namespace PhotostaxAiCurator.Services.AI;

/// <summary>
/// Structured prompt templates for GPT-4o Vision analysis.
/// </summary>
public static class OpenAiPrompts
{
    public const string SystemPrompt = """
        You are a photo metadata analyst specializing in scanned physical photographs.
        You will be shown scanned images from an Epson FastFoto scanner.
        Images may include:
        - The FRONT of a photo (original or color-enhanced scan)
        - The BACK of a photo (may contain handwriting, dates, stamps, stickers)

        Your job is to analyze ALL provided images and extract comprehensive metadata.
        Pay special attention to:
        1. Any text visible on the BACK of the photo (handwriting, dates, stamps)
        2. People in the photo (count, descriptions, approximate ages)
        3. Location clues (landmarks, signs, terrain, architecture)
        4. Time period clues (clothing, vehicles, technology, photo quality)
        5. Events or occasions (birthday cakes, holiday decorations, wedding attire)

        Return your analysis as a JSON object. Be thorough but honest about confidence.
        If you cannot determine something, use null and explain in review_reason.
        """;

    public const string UserPromptTemplate = """
        Analyze this scanned photo stack and return a JSON object with these fields:
        {{
          "title": "suggested short title",
          "description": "detailed scene description (2-3 sentences)",
          "caption": "one-line caption suitable for a photo album",
          "date_estimate": "YYYY-MM-DD if known, or decade like '1980s', or null",
          "date_source": "ocr_back|ocr_front|visual_cues|unknown",
          "date_confidence": 0.0 to 1.0,
          "era": "estimated decade like '1970s' or null",
          "people": [
            {{"name": null, "description": "brief description", "face_position": "left|center|right", "confidence": 0.9}}
          ],
          "people_count": integer,
          "places": ["named places visible or inferred"],
          "landmarks": [{{"name": "landmark name", "confidence": 0.9}}],
          "location_estimate": {{"latitude": number or null, "longitude": number or null, "place_name": "City, State/Country"}},
          "events": ["event type if detectable"],
          "holidays": ["holiday if detectable"],
          "mood": "emotional tone (joyful, solemn, candid, formal, etc.)",
          "scene": "scene classification (outdoor beach, indoor party, portrait, etc.)",
          "objects": ["notable objects"],
          "colors": ["dominant colors"],
          "ocr_front": "any text on the front or null",
          "ocr_back": "any text on the back or null",
          "handwriting_back": "transcription of handwriting on back or null",
          "overall_confidence": 0.0 to 1.0,
          "needs_human_review": ["field names where you're uncertain"],
          "review_reason": "brief explanation of what needs human input"
        }}

        {0}

        Respond ONLY with the JSON object, no markdown formatting.
        """;

    public static string GetContextNote(Dictionary<string, string>? existingMetadata)
    {
        if (existingMetadata is null || existingMetadata.Count == 0)
            return "No existing metadata is available.";

        var lines = existingMetadata.Select(kv => $"  {kv.Key}: {kv.Value}");
        return $"Existing metadata from the scanner:\n{string.Join("\n", lines)}";
    }
}
