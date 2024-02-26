using CommunityToolkit.Mvvm.ComponentModel;
using DigitClassifier.Interfaces;
using DigitClassifier.ViewModels;
using DigitClassifier.Views;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<ImagesViewModel, ImagesPage>();
        Configure<NetworksViewModel, NetworksPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}");
            }
        }

        return pageType;
    }

    private void Configure<ViewModel, View>() where ViewModel : ObservableObject where View : Page
    {
        lock (_pages)
        {
            var key = typeof(ViewModel).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(View);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
