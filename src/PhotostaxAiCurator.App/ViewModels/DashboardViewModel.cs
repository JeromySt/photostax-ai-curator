using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotostaxAiCurator.App.Views;
using PhotostaxAiCurator.Domain.Interfaces;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IPhotostaxAdapter _photostax;
    private readonly IReviewQueueService _reviewQueue;
    private readonly IEnrichmentPipeline _pipeline;

    [ObservableProperty] private int _totalStacks;
    [ObservableProperty] private int _analyzed;
    [ObservableProperty] private int _needsReview;
    [ObservableProperty] private int _written;
    [ObservableProperty] private double _overallProgress;
    [ObservableProperty] private string _progressText = "Ready to analyze";

    public DashboardViewModel(
        IPhotostaxAdapter photostax,
        IReviewQueueService reviewQueue,
        IEnrichmentPipeline pipeline)
    {
        _photostax = photostax;
        _reviewQueue = reviewQueue;
        _pipeline = pipeline;
    }

    [RelayCommand]
    private async Task StartAnalysisAsync()
    {
        await Shell.Current.GoToAsync(nameof(BatchProgressPage));
    }

    [RelayCommand]
    private async Task GoToReviewAsync()
    {
        await Shell.Current.GoToAsync("//ReviewPage");
    }

    [RelayCommand]
    private async Task WriteAllApprovedAsync()
    {
        var approved = _reviewQueue.GetAll()
            .Where(i => i.Status == Domain.Models.EnrichmentStatus.Approved)
            .ToList();

        if (approved.Count == 0)
        {
            await Shell.Current.DisplayAlertAsync("Nothing to Write", "No approved items to write.", "OK");
            return;
        }

        foreach (var item in approved)
        {
            await _pipeline.WriteMetadataAsync(item);
            Written++;
        }

        await Shell.Current.DisplayAlertAsync("Done", $"Wrote metadata for {approved.Count} stacks.", "OK");
    }
}
