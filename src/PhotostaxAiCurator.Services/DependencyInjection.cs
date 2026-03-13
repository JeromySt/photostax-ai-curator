using Microsoft.Extensions.DependencyInjection;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Services.AI;
using PhotostaxAiCurator.Services.Pipeline;

namespace PhotostaxAiCurator.Services;

/// <summary>
/// Extension methods for registering services in DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAiCuratorServices(this IServiceCollection services)
    {
        services.AddHttpClient<IAiVisionService, OpenAiVisionService>();
        services.AddSingleton<IReviewQueueService, ReviewQueueService>();
        services.AddTransient<IEnrichmentPipeline, EnrichmentPipeline>();
        return services;
    }
}
