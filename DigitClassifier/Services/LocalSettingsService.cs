using DigitClassifier.Helpers;
using DigitClassifier.Interfaces;
using DigitClassifier.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Windows.Storage;

namespace DigitClassifier.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private const string _defaultApplicationDataFolder = "DigitClassifier/ApplicationData";
    private const string _defaultLocalSettingsFile = "LocalSettings.json";

    private readonly LocalSettingsOptions _options;

    private readonly string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string _applicationDataFolder;
    private readonly string _localSettingsFile;
    private IDictionary<string, object> _settings;

    private bool _isInitialized;

    public LocalSettingsService(IOptions<LocalSettingsOptions> options)
    {
        _options = options.Value;

        _applicationDataFolder = Path.Combine(_localApplicationData, _options.ApplicationDataFolder ?? _defaultApplicationDataFolder);
        _localSettingsFile = _options.LocalSettingsFile ?? _defaultLocalSettingsFile;

        _settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            await Task.Run(() =>
            {
                _settings = new Dictionary<string, object>();

                var path = Path.Combine(_applicationDataFolder, _localSettingsFile);
                if (File.Exists(path))
                {
                    var json = JsonSerializer.Deserialize<IDictionary<string, object>>(File.ReadAllText(path));

                    if (json != null)
                        _settings = json;
                }
            });

            _isInitialized = true;
        }
    }

    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return JsonSerializer.Deserialize<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (_settings != null && _settings.TryGetValue(key, out var obj))
            {
                return JsonSerializer.Deserialize<T>((string)obj);
            }
        }

        return default;
    }

    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = JsonSerializer.Serialize(value);
        }
        else
        {
            await InitializeAsync();

            _settings[key] = JsonSerializer.Serialize(value);

            await Task.Run(() =>
            {
                if (!Directory.Exists(_applicationDataFolder))
                {
                    Directory.CreateDirectory(_applicationDataFolder);
                }

                var fileContent = JsonSerializer.Serialize(_settings);
                File.WriteAllText(Path.Combine(_applicationDataFolder, _localSettingsFile), fileContent, Encoding.UTF8);
            });
        }
    }
}