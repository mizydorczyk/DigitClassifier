using DigitClassifier.Helpers;
using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; private set; }

        public ShellPage(ShellViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();

            ViewModel.NavigationService.Frame = NavigationFrame;
            ViewModel.NotificationService.Initialize(ApplicationInfoBar);
            ViewModel.NavigationViewService.Initialize(NavigationViewControl);

            App.MainWindow.ExtendsContentIntoTitleBar = true;
            App.MainWindow.SetTitleBar(AppTitleBar);
            ApplicationTitle.Text = "ApplicationTitle".GetLocalized();
        }
    }
}