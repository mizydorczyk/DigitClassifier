using DigitClassifier.Extensions;
using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DigitClassifier.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IPageService _pageService;
        private Frame? _frame;

        public event NavigatedEventHandler? Navigated;

        public Frame? Frame
        {
            get
            {
                if (_frame == null)
                {
                    _frame = App.MainWindow.Content as Frame;
                    RegisterFrameEvents();
                }

                return _frame;
            }

            set
            {
                UnregisterFrameEvents();
                _frame = value;
                RegisterFrameEvents();
            }
        }

        public NavigationService(IPageService pageService)
        {
            _pageService = pageService;
        }

        private void RegisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (_frame != null)
            {
                _frame.Navigated -= OnNavigated;
            }
        }

        public bool NavigateTo(string pageKey, object? parameter = null)
        {
            var pageType = _pageService.GetPageType(pageKey);

            if (_frame != null && _frame.Content?.GetType() != pageType)
            {
                var vmBeforeNavigation = _frame.GetPageViewModel();
                var navigated = _frame.Navigate(pageType, parameter);
                if (navigated)
                {
                    if (vmBeforeNavigation is INavigationAware navigationAware)
                    {
                        navigationAware.OnNavigatedFrom();
                    }
                }

                return navigated;
            }

            return false;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame)
            {
                if (frame.GetPageViewModel() is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedTo(e.Parameter);
                }

                Navigated?.Invoke(sender, e);
            }
        }
    }
}
