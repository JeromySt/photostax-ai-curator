using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;
using PhotostaxAiCurator.Domain.Configuration;

namespace PhotostaxAiCurator.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly AiCuratorOptions _options;

    [ObservableProperty] private string _apiKey = string.Empty;
    [ObservableProperty] private string _selectedModel = "gpt-4o";
    [ObservableProperty] private double _autoApproveThreshold = 0.85;
    [ObservableProperty] private double _mediumThreshold = 0.50;
    [ObservableProperty] private int _maxConcurrency = 3;
    [ObservableProperty] private double _maxConcurrencyDouble = 3.0;
    [ObservableProperty] private string? _statusMessage;
    [ObservableProperty] private bool _hasStatusMessage;

    public List<string> AvailableModels { get; } = ["gpt-4o", "gpt-4o-mini", "gpt-4-turbo"];

    public SettingsViewModel(IOptions<AiCuratorOptions> options)
    {
        _options = options.Value;
    }

    public async void LoadSettings()
    {
        try
        {
            ApiKey = await SecureStorage.GetAsync("openai_api_key") ?? string.Empty;
        }
        catch
        {
            ApiKey = string.Empty;
        }

        SelectedModel = _options.OpenAiModel;
        AutoApproveThreshold = _options.AutoApproveThreshold;
        MediumThreshold = _options.MediumConfidenceThreshold;
        MaxConcurrency = _options.MaxConcurrency;
        MaxConcurrencyDouble = _options.MaxConcurrency;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            await SecureStorage.SetAsync("openai_api_key", ApiKey);

            _options.OpenAiApiKey = ApiKey;
            _options.OpenAiModel = SelectedModel;
            _options.AutoApproveThreshold = AutoApproveThreshold;
            _options.MediumConfidenceThreshold = MediumThreshold;
            _options.MaxConcurrency = (int)MaxConcurrencyDouble;

            StatusMessage = "✅ Settings saved!";
            HasStatusMessage = true;

            await Task.Delay(3000);
            HasStatusMessage = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
            HasStatusMessage = true;
        }
    }
}
