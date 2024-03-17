using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using DigitClassifier.Services;
using Microsoft.UI.Xaml.Controls;
using NeuralNetwork;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DigitClassifier.ViewModels
{
    public partial class NetworksViewModel : ObservableRecipient, INavigationAware
    {
        private readonly INetworksService _networksService;
        private readonly INotificationService _notificationService;

        public ObservableCollection<Network> Networks { get; private set; } = new();
        public ICommand ItemSelectedCommand { get; }
        public ICommand ItemDeletedCommand { get; }
        public ICommand ItemCreatedCommand { get; }
        private CancellationTokenSource? _loadingCancellationToken;

        [ObservableProperty] Network _activeNetwork;

        public NetworksViewModel(INetworksService networksService, INotificationService notificationService)
        {
            _networksService = networksService;
            _notificationService = notificationService;

            ItemSelectedCommand = new RelayCommand<ListView>(OnItemSelected);
            ItemDeletedCommand = new RelayCommand<Button>(OnItemDeleted);
            ItemCreatedCommand = new RelayCommand<Network>(OnItemCreated);
        }

        public void OnNavigatedFrom()
        {
            _loadingCancellationToken?.Cancel();
        }

        public async void OnNavigatedTo(object parameter)
        {
            await LoadNetworks(true);
        }

        private async Task LoadNetworks(bool refresh = false)
        {
            Networks.Clear();

            _loadingCancellationToken?.Cancel();
            _loadingCancellationToken = new CancellationTokenSource();
            var loadingCancellationToken = _loadingCancellationToken.Token;

            try
            {
                var networks = await _networksService.GetNetworksAsync(refresh);
                ActiveNetwork = _networksService.ActiveNetwork;

                foreach (var network in networks)
                {
                    if (loadingCancellationToken.IsCancellationRequested)
                    {
                        Networks.Clear();
                        break;
                    }

                    Networks.Add(network);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }
        }

        private void OnItemSelected(ListView listView)
        {
            var selectedItem = listView.SelectedItem;

            if (selectedItem is Network)
            {
                _networksService.ActiveNetwork = (Network)selectedItem;
            }

            ActiveNetwork = _networksService.ActiveNetwork;
        }

        private async void OnItemDeleted(Button button)
        {
            var deletedItem = (Network)button.DataContext;

            try
            {
                await _networksService.DeleteNetworkAsync(deletedItem);
                await LoadNetworks(true);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }
        }

        private async void OnItemCreated(Network network)
        {
            if (network == null)
                return;

            try
            {
                await _networksService.SaveNetworkAsync(network);
                await LoadNetworks(true);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }
        }
    }
}