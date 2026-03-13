using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class StackDetailViewModel : ObservableObject
{
    private readonly IPhotostaxAdapter _photostax;
    private readonly IEnrichmentPipeline _pipeline;
    private StackInfo? _stack;
    private ReviewItem? _reviewItem;

    [ObservableProperty] private string _stackId = string.Empty;
    [ObservableProperty] private bool _hasOriginal;
    [ObservableProperty] private bool _hasEnhanced;
    [ObservableProperty] private bool _hasBack;
    [ObservableProperty] private ImageSource? _originalImageSource;
    [ObservableProperty] private ImageSource? _enhancedImageSource;
    [ObservableProperty] private ImageSource? _backImageSource;
    [ObservableProperty] private bool _hasAiResult;
    [ObservableProperty] private string? _aiTitle;
    [ObservableProperty] private string? _aiDescription;
    [ObservableProperty] private string? _aiDate;
    [ObservableProperty] private string? _aiPeople;
    [ObservableProperty] private string? _aiPlaces;
    [ObservableProperty] private string? _aiEvents;
    [ObservableProperty] private string? _aiOcrBack;
    [ObservableProperty] private string? _aiMood;
    [ObservableProperty] private double _aiConfidence;

    public ObservableCollection<KeyValuePair<string, string>> ExifTags { get; } = [];

    public StackDetailViewModel(IPhotostaxAdapter photostax, IEnrichmentPipeline pipeline)
    {
        _photostax = photostax;
        _pipeline = pipeline;
    }

    public void LoadStack(string stackId)
    {
        StackId = stackId;
        _stack = _photostax.GetStackWithMetadata(stackId);

        HasOriginal = _stack.OriginalPath is not null;
        HasEnhanced = _stack.EnhancedPath is not null;
        HasBack = _stack.BackPath is not null;

        if (_stack.OriginalPath is not null)
            OriginalImageSource = ImageSource.FromFile(_stack.OriginalPath);
        if (_stack.EnhancedPath is not null)
            EnhancedImageSource = ImageSource.FromFile(_stack.EnhancedPath);
        if (_stack.BackPath is not null)
            BackImageSource = ImageSource.FromFile(_stack.BackPath);

        ExifTags.Clear();
        foreach (var tag in _stack.ExifTags)
            ExifTags.Add(tag);
    }

    [RelayCommand]
    private async Task AnalyzeAsync()
    {
        try
        {
            _reviewItem = await _pipeline.AnalyzeStackAsync(StackId);
            var ai = _reviewItem.AiResult;

            HasAiResult = true;
            AiTitle = ai.Title;
            AiDescription = ai.Description;
            AiDate = ai.DateEstimate;
            AiPeople = ai.People.Count > 0 ? string.Join(", ", ai.People.Select(p => p.Name ?? p.Description)) : null;
            AiPlaces = ai.Places.Count > 0 ? string.Join(", ", ai.Places) : null;
            AiEvents = ai.Events.Count > 0 ? string.Join(", ", ai.Events) : null;
            AiOcrBack = ai.OcrBack;
            AiMood = ai.Mood;
            AiConfidence = ai.OverallConfidence;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Analysis Error", ex.Message, "OK");
        }
    }
}
