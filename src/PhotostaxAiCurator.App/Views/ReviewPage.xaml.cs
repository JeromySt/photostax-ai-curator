using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class ReviewPage : ContentPage
{
    public ReviewPage(ReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ReviewViewModel vm)
            vm.LoadQueue();
    }
}
