using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class ScanPage : ContentPage
{
    public ScanPage(ScanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
