using PhotostaxAiCurator.App.ViewModels;

namespace PhotostaxAiCurator.App.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SettingsViewModel vm)
            vm.LoadSettings();
    }
}
