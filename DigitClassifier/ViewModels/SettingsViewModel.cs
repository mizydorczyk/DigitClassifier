using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using Windows.Storage.Pickers;

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
        public ICommand SettingChangedCommand { get; }
        public ICommand OpenFolderCommand { get; }
        public ICommand OpenFileCommand { get; }

        public SettingsViewModel(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;

            SettingChangedCommand = new RelayCommand<TextBox>(OnSettingChangedCommand);
            OpenFolderCommand = new RelayCommand<string>(OnOpenFolderCommand);
            OpenFileCommand = new RelayCommand<string>(OnOpenFileCommand);
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

        private async void OnSettingChangedCommand(TextBox textBox)
        {
            if (textBox == null)
                return;

            var name = textBox.Name;
            var value = textBox.Text;

            await _localSettingsService.SaveSettingAsync(name, value);
        }

        private async void OnOpenFolderCommand(string setting)
        {
            if (string.IsNullOrEmpty(setting))
                return;

            var openPicker = new FolderPicker();
            var window = App.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add("*");

            var folder = await openPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                await _localSettingsService.SaveSettingAsync(setting, folder.Path);
                await LoadSettings();
            }
        }

        private async void OnOpenFileCommand(string setting)
        {
            if (string.IsNullOrEmpty(setting))
                return;

            var openPicker = new FileOpenPicker();
            var window = App.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add("*");

            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                await _localSettingsService.SaveSettingAsync(setting, file.Path);
                await LoadSettings();
            }
        }
    }
}