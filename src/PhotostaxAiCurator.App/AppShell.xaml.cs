using PhotostaxAiCurator.App.Views;

namespace PhotostaxAiCurator.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(StackDetailPage), typeof(StackDetailPage));
        Routing.RegisterRoute(nameof(BatchProgressPage), typeof(BatchProgressPage));
    }
}
