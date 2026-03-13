using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Domain.Models;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class BatchProgressViewModel : ObservableObject
{
    private readonly IEnrichmentPipeline _pipeline;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private double _progress;
    [ObservableProperty] private string _progressText = "Starting...";
    [ObservableProperty] private string? _currentStackId;
    [ObservableProperty] private int _autoApproved;
    [ObservableProperty] private int _pendingReview;
    [ObservableProperty] private int _failed;

    public BatchProgressViewModel(IEnrichmentPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public async void StartAnalysis()
    {
        _cts = new CancellationTokenSource();
        var progressHandler = new Progress<BatchProgress>(OnProgress);

        try
        {
            await _pipeline.RunBatchAsync(progressHandler, _cts.Token);
            ProgressText = "Analysis complete!";
        }
        catch (OperationCanceledException)
        {
            ProgressText = "Analysis cancelled.";
        }
        catch (Exception ex)
        {
            ProgressText = $"Error: {ex.Message}";
        }
    }

    private void OnProgress(BatchProgress p)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Progress = p.TotalStacks > 0 ? (double)p.Completed / p.TotalStacks : 0;
            ProgressText = $"{p.Completed} of {p.TotalStacks} ({p.PercentComplete:F0}%)";
            CurrentStackId = p.CurrentStackId;
            AutoApproved = p.AutoApproved;
            PendingReview = p.NeedsReview;
            Failed = p.Failed;
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        _cts?.Cancel();
        await Shell.Current.GoToAsync("..");
    }
}
