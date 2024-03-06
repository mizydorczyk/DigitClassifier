using CommunityToolkit.Mvvm.ComponentModel;
using DigitClassifier.Interfaces;

namespace DigitClassifier.ViewModels
{
    public partial class SettingsViewModel : ObservableRecipient, INavigationAware
    {
        private readonly ILocalSettingsService _localSettingsService;
        [ObservableProperty] private string? _trainingLabelsFile;
        [ObservableProperty] private string? _trainingImagesFile;
        [ObservableProperty] private string? _testLabelsFile;
        [ObservableProperty] private string? _testImagesFile;
        [ObservableProperty] private string? _networksFolder;

        public SettingsViewModel(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public void OnNavigatedFrom()
        {
            return;
        }

        public async void OnNavigatedTo(object parameter)
        {
            await LoadSettings();
        }

        private async Task LoadSettings()
        {
            TrainingLabelsFile = await _localSettingsService.ReadSettingAsync<string>("TrainingLabelsFile");
            TrainingImagesFile = await _localSettingsService.ReadSettingAsync<string>("TrainingImagesFile");
            TestLabelsFile = await _localSettingsService.ReadSettingAsync<string>("TestLabelsFile");
            TestImagesFile = await _localSettingsService.ReadSettingAsync<string>("TestImagesFile");
            NetworksFolder = await _localSettingsService.ReadSettingAsync<string>("NetworksFolder");
        }
    }
}