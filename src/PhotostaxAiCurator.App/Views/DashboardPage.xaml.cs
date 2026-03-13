using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
