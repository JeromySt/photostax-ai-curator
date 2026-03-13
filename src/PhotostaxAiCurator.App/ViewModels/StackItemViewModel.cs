using CommunityToolkit.Mvvm.ComponentModel;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class StackItemViewModel : ObservableObject
{
    public string Id { get; }
    public string? OriginalPath { get; }
    public string? EnhancedPath { get; }
    public string? BackPath { get; }
    public bool HasBack { get; }

    [ObservableProperty]
    private EnrichmentStatus _status;

    public ImageSource? ThumbnailSource =>
        EnhancedPath is not null ? ImageSource.FromFile(EnhancedPath)
        : OriginalPath is not null ? ImageSource.FromFile(OriginalPath)
        : null;

    public string StatusText => Status switch
    {
        EnrichmentStatus.Pending => "Not analyzed",
        EnrichmentStatus.Analyzing => "Analyzing...",
        EnrichmentStatus.AutoApproved => "✅ Auto-approved",
        EnrichmentStatus.NeedsReview => "⚠️ Needs review",
        EnrichmentStatus.Approved => "✅ Approved",
        EnrichmentStatus.Written => "💾 Written",
        EnrichmentStatus.Failed => "❌ Failed",
        EnrichmentStatus.Skipped => "⏭ Skipped",
        _ => ""
    };

    public Color StatusColor => Status switch
    {
        EnrichmentStatus.AutoApproved or EnrichmentStatus.Approved or EnrichmentStatus.Written => Colors.Green,
        EnrichmentStatus.NeedsReview => Colors.Orange,
        EnrichmentStatus.Failed => Colors.Red,
        _ => Colors.Gray,
    };

    public StackItemViewModel(StackInfo stack)
    {
        Id = stack.Id;
        OriginalPath = stack.OriginalPath;
        EnhancedPath = stack.EnhancedPath;
        BackPath = stack.BackPath;
        HasBack = stack.HasBack;
        _status = stack.EnrichmentStatus;
    }
}
