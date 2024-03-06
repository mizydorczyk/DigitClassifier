using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DigitClassifier.Interfaces
{
    public interface INavigationService
    {
        event NavigatedEventHandler Navigated;
        Frame? Frame { get; set; }
        bool NavigateTo(string pageKey, object? parameter = null);
    }
}