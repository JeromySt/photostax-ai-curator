using Microsoft.Extensions.DependencyInjection;
using PhotostaxAiCurator.Domain.Interfaces;
using PhotostaxAiCurator.Infrastructure.Photostax;

namespace PhotostaxAiCurator.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services in DI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAiCuratorInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPhotostaxAdapter, PhotostaxAdapter>();
        return services;
    }
}
