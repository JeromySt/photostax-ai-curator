using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class BatchProgressPage : ContentPage
{
    public BatchProgressPage(BatchProgressViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BatchProgressViewModel vm)
            vm.StartAnalysis();
    }
}
