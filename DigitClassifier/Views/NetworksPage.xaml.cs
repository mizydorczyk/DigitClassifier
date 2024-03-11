using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using NeuralNetwork;
using Serilog;

namespace DigitClassifier.Views
{
    public sealed partial class NetworksPage : Page
    {
        public NetworksViewModel ViewModel { get; private set; }

        public NetworksPage()
        {
            ViewModel = App.GetService<NetworksViewModel>();
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ItemSelectedCommand.Execute(sender);
        }

        private void ListViewItemContainer_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsShown", true);
        }

        private void ListViewItemContainer_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Control, "HoverButtonsHidden", true);
        }

        private async void HoverButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Delete the network permanently?",
                Content = "If you delete this network, you won't be able to recover it. Do you want to delete it?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel"
            };

            if (await dialog.ShowAsync() is ContentDialogResult.Primary)
                ViewModel.ItemDeletedCommand.Execute(sender);
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            NetworksList.SelectedItem = ViewModel.ActiveNetwork;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var createNetworkDialogPage = new CreateNetworkDialogPage();
            ContentDialog dialog = new ContentDialog
            {
                XamlRoot = XamlRoot,
                Title = "Create new network",
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                IsPrimaryButtonEnabled = false,
                Content = createNetworkDialogPage
            };

            createNetworkDialogPage.LayersTextChanged += (sender, e) => { dialog.IsPrimaryButtonEnabled = createNetworkDialogPage.IsValid(); };

            dialog.PrimaryButtonClick += (sender, args) =>
            {
                if (!createNetworkDialogPage.IsValid())
                {
                    args.Cancel = true;
                }
            };

            if (await dialog.ShowAsync() is ContentDialogResult.Primary)
            {
                try
                {
                    var layers = createNetworkDialogPage.GetLayersInput().Select(int.Parse);
                    var activationFunction = createNetworkDialogPage.GetActivationFunctionInput();

                    ViewModel.ItemCreatedCommand.Execute(new Network([.. layers], activationFunction));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex.ToString());
                }
            }
        }
    }
}