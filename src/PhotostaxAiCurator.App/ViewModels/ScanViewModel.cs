using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhotostaxAiCurator.App.Views;
using PhotostaxAiCurator.Domain.Interfaces;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class ScanViewModel : ObservableObject
{
    private readonly IPhotostaxAdapter _photostax;

    [ObservableProperty] private string _folderPath = string.Empty;
    [ObservableProperty] private int _totalStacks;
    [ObservableProperty] private int _stacksWithBack;
    [ObservableProperty] private bool _hasStacks;

    public ObservableCollection<StackItemViewModel> Stacks { get; } = [];

    public ScanViewModel(IPhotostaxAdapter photostax)
    {
        _photostax = photostax;
    }

    [RelayCommand]
    private async Task BrowseFolderAsync()
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful)
            {
                FolderPath = result.Folder.Path;
                await ScanFolderAsync();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to pick folder: {ex.Message}", "OK");
        }
    }

    private async Task ScanFolderAsync()
    {
        try
        {
            _photostax.Open(FolderPath, recursive: true);
            var stacks = _photostax.Scan();

            Stacks.Clear();
            foreach (var stack in stacks)
                Stacks.Add(new StackItemViewModel(stack));

            TotalStacks = stacks.Count;
            StacksWithBack = stacks.Count(s => s.HasBack);
            HasStacks = stacks.Count > 0;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Scan Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task StackSelectedAsync(object? selectedItem)
    {
        if (selectedItem is StackItemViewModel stack)
        {
            await Shell.Current.GoToAsync($"{nameof(StackDetailPage)}?stackId={stack.Id}");
        }
    }
}
