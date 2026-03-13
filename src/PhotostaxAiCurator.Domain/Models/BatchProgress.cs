namespace PhotostaxAiCurator.Domain.Models;

/// <summary>
/// Real-time progress snapshot of a batch analysis operation.
/// </summary>
public sealed class BatchProgress
{
    public int TotalStacks { get; set; }
    public int Completed { get; set; }
    public int AutoApproved { get; set; }
    public int NeedsReview { get; set; }
    public int Failed { get; set; }
    public string? CurrentStackId { get; set; }
    public bool IsCancelled { get; set; }
    public bool IsPaused { get; set; }

    public double PercentComplete => TotalStacks > 0
        ? (double)Completed / TotalStacks * 100.0
        : 0;
}
