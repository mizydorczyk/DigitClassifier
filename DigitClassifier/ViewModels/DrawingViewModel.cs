using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using DigitClassifier.Models;
using Serilog;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.ViewModels
{
    public partial class DrawingViewModel : ObservableRecipient, INavigationAware
    {
        private readonly INetworksService _networksService;
        private readonly INotificationService _notificationService;
        public ObservableCollection<CalculatedResult> CalculatedResults = new();
        public ICommand DrawingChangedCommand { get; }

        public DrawingViewModel(INetworksService networksService, INotificationService notificationService)
        {
            _networksService = networksService;
            _notificationService = notificationService;
            DrawingChangedCommand = new RelayCommand<double[]>(OnDrawingChangedCommand);
        }

        private void OnDrawingChangedCommand(double[] inputs)
        {
            if (_networksService.ActiveNetwork == null)
                return;

            var calculatedResults = _networksService.ActiveNetwork.Calculate(inputs).Select((value, index) => new CalculatedResult(index, value));
            calculatedResults = [.. calculatedResults.OrderByDescending(x => x.Result)];

            foreach (var calculatedResult in calculatedResults)
            {
                var item = CalculatedResults.FirstOrDefault(x => x.Label == calculatedResult.Label);
                if (item != null)
                    CalculatedResults.Remove(item);

                CalculatedResults.Add(calculatedResult);
            }
        }

        public void OnNavigatedFrom()
        {
            return;
        }

        public async void OnNavigatedTo(object parameter)
        {
            await LoadNetworks(true);
        }

        private async Task LoadNetworks(bool refresh = false)
        {
            try
            {
                var networks = await _networksService.GetNetworksAsync(refresh);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }
        }
    }
}