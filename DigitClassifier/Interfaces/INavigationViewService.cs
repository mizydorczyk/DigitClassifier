using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Interfaces
{
    public interface INavigationViewService
    {
        void Initialize(NavigationView navigationView);
        NavigationViewItem? GetSelectedItem(Type pageType);
    }
}