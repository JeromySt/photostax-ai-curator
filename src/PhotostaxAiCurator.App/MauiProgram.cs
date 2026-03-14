using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PhotostaxAiCurator.App.ViewModels;
using PhotostaxAiCurator.App.Views;
using PhotostaxAiCurator.Domain.Configuration;
using PhotostaxAiCurator.Infrastructure;
using PhotostaxAiCurator.Services;

namespace PhotostaxAiCurator.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuration
        builder.Services.Configure<AiCuratorOptions>(options =>
        {
            // Defaults are in AiCuratorOptions; API key loaded from SecureStorage at runtime
        });

        // Register layers
        builder.Services.AddAiCuratorServices();
        builder.Services.AddAiCuratorInfrastructure();

        // Register ViewModels
        builder.Services.AddSingleton<WorkspaceViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Register Pages
        builder.Services.AddTransient<WorkspacePage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
