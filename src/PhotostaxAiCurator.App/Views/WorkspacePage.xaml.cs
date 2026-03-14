using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class WorkspacePage : ContentPage
{
    public WorkspacePage(WorkspaceViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
