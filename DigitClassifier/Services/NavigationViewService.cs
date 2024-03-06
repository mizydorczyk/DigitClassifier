using DigitClassifier.Helpers;
using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Services
{
    public class NavigationViewService : INavigationViewService
    {
        private NavigationView? _navigationView;
        private readonly INavigationService _navigationService;
        private readonly IPageService _pageService;

        public NavigationViewService(INavigationService navigationService, IPageService pageService)
        {
            _navigationService = navigationService;
            _pageService = pageService;
        }

        public void Initialize(NavigationView navigationView)
        {
            _navigationView = navigationView;
            _navigationView.ItemInvoked += OnItemInvoked;
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.InvokedItemContainer;

            if (selectedItem?.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                _navigationService.NavigateTo(pageKey);
            }
        }

        public NavigationViewItem? GetSelectedItem(Type pageType)
        {
            foreach (var item in _navigationView.MenuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            if (menuItem.GetValue(NavigationHelper.NavigateToProperty) is string pageKey)
            {
                return _pageService.GetPageType(pageKey) == sourcePageType;
            }

            return false;
        }
    }
}