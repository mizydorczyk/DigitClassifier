using CommunityToolkit.Mvvm.ComponentModel;
using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;

namespace DigitClassifier.ViewModels
{
    public partial class ShellViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private object? selected;

        public INavigationService NavigationService { get; private set; }
        public INavigationViewService NavigationViewService { get; private set; }

        public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService)
        {
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;

            NavigationViewService = navigationViewService;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }
    }
}
