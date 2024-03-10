using DigitClassifier.Activation;
using DigitClassifier.Interfaces;
using DigitClassifier.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly ILocalSettingsService _localSettingsService;
        private UIElement? _shell = null;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers,
            ILocalSettingsService localSettingsService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _localSettingsService = localSettingsService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (App.MainWindow.Content == null)
            {
                _shell = App.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
            }

            await HandleActivationAsync(activationArgs);
            await InitializeSettingsAsync();

            App.MainWindow.Activate();
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
        }

        private async Task InitializeSettingsAsync()
        {
            var options = new string[]
            {
                "TrainingImagesFile",
                "TrainingLabelsFile",
                "TestImagesFile",
                "TestLabelsFile",
                "NetworksFolder"
            };

            foreach (var option in options)
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;
                if (string.IsNullOrEmpty(await _localSettingsService.ReadSettingAsync<string>(option)))
                {
                    var defaultValue = (option) switch
                    {
                        "TrainingImagesFile" => Path.Combine(root, @"Assets\train-images.idx3-ubyte"),
                        "TrainingLabelsFile" => Path.Combine(root, @"Assets\train-labels.idx1-ubyte"),
                        "TestImagesFile" => Path.Combine(root, @"Assets\t10k-images.idx3-ubyte"),
                        "TestLabelsFile" => Path.Combine(root, @"Assets\t10k-labels.idx1-ubyte"),
                        "NetworksFolder" => Path.Combine(root, @"Assets\Networks"),
                        _ => string.Empty
                    };

                    await _localSettingsService.SaveSettingAsync<string>(option, defaultValue);
                }
            }
        }
    }
}