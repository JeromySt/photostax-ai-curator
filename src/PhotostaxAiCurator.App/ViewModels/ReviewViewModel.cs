using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class ReviewViewModel : ObservableObject
{
    private readonly IReviewQueueService _reviewQueue;
    private readonly IPhotostaxAdapter _photostax;
    private readonly IEnrichmentPipeline _pipeline;
    private ReviewItem? _currentItem;

    [ObservableProperty] private int _queueCount;
    [ObservableProperty] private int _currentIndex;
    [ObservableProperty] private bool _hasCurrentItem;
    [ObservableProperty] private bool _isQueueEmpty = true;
    [ObservableProperty] private ImageSource? _frontImageSource;
    [ObservableProperty] private ImageSource? _backImageSource;
    [ObservableProperty] private bool _hasBackImage;

    // Editable fields
    [ObservableProperty] private string? _editableTitle;
    [ObservableProperty] private string? _editableDescription;
    [ObservableProperty] private string? _editableDate;
    [ObservableProperty] private string? _editablePeople;
    [ObservableProperty] private string? _editablePlaces;
    [ObservableProperty] private string? _editableEvents;
    [ObservableProperty] private string? _editableOcrBack;

    // Confidence display
    [ObservableProperty] private string _confidenceText = string.Empty;
    [ObservableProperty] private Color _confidenceBadgeColor = Colors.Gray;
    [ObservableProperty] private string? _reviewReason;
    [ObservableProperty] private bool _hasReviewReason;

    public ReviewViewModel(IReviewQueueService reviewQueue, IPhotostaxAdapter photostax, IEnrichmentPipeline pipeline)
    {
        _reviewQueue = reviewQueue;
        _photostax = photostax;
        _pipeline = pipeline;
    }

    public void LoadQueue()
    {
        QueueCount = _reviewQueue.Count;
        IsQueueEmpty = QueueCount == 0;
        CurrentIndex = 1;

        if (!IsQueueEmpty)
            LoadNextItem();
    }

    private void LoadNextItem()
    {
        _currentItem = _reviewQueue.Dequeue();
        if (_currentItem is null)
        {
            HasCurrentItem = false;
            IsQueueEmpty = true;
            return;
        }

        HasCurrentItem = true;
        IsQueueEmpty = false;

        var ai = _currentItem.AiResult;
        EditableTitle = ai.Title;
        EditableDescription = ai.Description;
        EditableDate = ai.DateEstimate;
        EditablePeople = ai.People.Count > 0 ? string.Join(", ", ai.People.Select(p => p.Name ?? p.Description)) : null;
        EditablePlaces = ai.Places.Count > 0 ? string.Join(", ", ai.Places) : null;
        EditableEvents = ai.Events.Count > 0 ? string.Join(", ", ai.Events) : null;
        EditableOcrBack = ai.OcrBack;

        ReviewReason = ai.ReviewReason;
        HasReviewReason = ai.ReviewReason is not null;

        ConfidenceText = $"Confidence: {ai.OverallConfidence:P0}";
        ConfidenceBadgeColor = _currentItem.Confidence switch
        {
            ConfidenceLevel.High => Colors.Green,
            ConfidenceLevel.Medium => Colors.Orange,
            ConfidenceLevel.Low => Colors.Red,
            _ => Colors.Gray,
        };

        // Load images
        try
        {
            var stack = _photostax.GetStackWithMetadata(_currentItem.StackId);
            var imagePath = stack.EnhancedPath ?? stack.OriginalPath;
            FrontImageSource = imagePath is not null ? ImageSource.FromFile(imagePath) : null;
            HasBackImage = stack.BackPath is not null;
            BackImageSource = stack.BackPath is not null ? ImageSource.FromFile(stack.BackPath) : null;
        }
        catch
        {
            FrontImageSource = null;
            BackImageSource = null;
            HasBackImage = false;
        }
    }

    [RelayCommand]
    private async Task ApproveAsync()
    {
        if (_currentItem is null) return;
        _reviewQueue.Approve(_currentItem.StackId);
        await _pipeline.WriteMetadataAsync(_currentItem);
        MoveToNext();
    }

    [RelayCommand]
    private async Task ApproveWithEditsAsync()
    {
        if (_currentItem is null) return;

        // Apply edits as user overrides
        if (EditableTitle != _currentItem.AiResult.Title)
            _currentItem.UserOverrides["title"] = EditableTitle;
        if (EditableDescription != _currentItem.AiResult.Description)
            _currentItem.UserOverrides["description"] = EditableDescription;
        if (EditableDate != _currentItem.AiResult.DateEstimate)
            _currentItem.UserOverrides["date"] = EditableDate;

        _reviewQueue.Approve(_currentItem.StackId);
        await _pipeline.WriteMetadataAsync(_currentItem);
        MoveToNext();
    }

    [RelayCommand]
    private void Skip()
    {
        if (_currentItem is null) return;
        _reviewQueue.Skip(_currentItem.StackId);
        MoveToNext();
    }

    private void MoveToNext()
    {
        CurrentIndex++;
        QueueCount = _reviewQueue.Count;
        LoadNextItem();
    }
}
