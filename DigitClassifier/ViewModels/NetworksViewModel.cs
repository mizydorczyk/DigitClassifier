using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
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
        public ObservableCollection<Network> Networks { get; private set; } = new ObservableCollection<Network>();
        public ICommand ItemSelectedCommand { get; }
        public ICommand ItemDeletedCommand { get; }
        private CancellationTokenSource? _loadingCancellationToken;

        [ObservableProperty] Network _activeNetwork;

        public NetworksViewModel(INetworksService networksService)
        {
            _networksService = networksService;

            ItemSelectedCommand = new RelayCommand<object>(OnItemSelected);
            ItemDeletedCommand = new RelayCommand<object>(OnItemDeleted);
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
            }
        }

        private void OnItemSelected(object args)
        {
            var listView = (ListView)args;
            var selectedItem = listView.SelectedItem;

            if (selectedItem is Network)
            {
                _networksService.ActiveNetwork = (Network)selectedItem;
            }

            ActiveNetwork = _networksService.ActiveNetwork;
        }

        private async void OnItemDeleted(object args)
        {
            var button = (Button)args;
            var deletedItem = (Network)button.DataContext;

            await _networksService.DeleteNetworkAsync(deletedItem);
            Networks.Clear();
            await LoadNetworks(true);
        }
    }
}