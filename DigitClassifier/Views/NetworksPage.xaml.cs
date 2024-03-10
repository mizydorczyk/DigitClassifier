using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

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
            ContentDialog dialog = new ContentDialog();

            dialog.XamlRoot = XamlRoot;
            dialog.Title = "Are you sure you want to delete the item?";
            dialog.PrimaryButtonText = "Yes";
            dialog.CloseButtonText = "No";
            dialog.DefaultButton = ContentDialogButton.Primary;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
                ViewModel.ItemDeletedCommand.Execute(sender);
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            NetworksList.SelectedItem = ViewModel.ActiveNetwork;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}