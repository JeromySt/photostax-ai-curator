using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

[QueryProperty(nameof(StackId), "stackId")]
public partial class StackDetailPage : ContentPage
{
    private readonly StackDetailViewModel _viewModel;

    public string StackId
    {
        set => _viewModel.LoadStack(value);
    }

    public StackDetailPage(StackDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
}
