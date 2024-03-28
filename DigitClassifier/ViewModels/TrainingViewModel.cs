using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System.Windows.Input;

namespace DigitClassifier.ViewModels
{
    public partial class TrainingViewModel : ObservableRecipient, INavigationAware
    {
        private readonly INetworksService _networksService;
        private readonly IImagesService _imagesService;
        private readonly INotificationService _notificationService;
        public ICommand TrainClickedCommand { get; }
        public event EventHandler TrainingEnded;

        public TrainingViewModel(INetworksService networksService, IImagesService imagesService, INotificationService notificationService)
        {
            _networksService = networksService;
            _imagesService = imagesService;
            _notificationService = notificationService;

            TrainClickedCommand = new RelayCommand<string[]>(OnTrainClickedCommand);
        }

        private async void OnTrainClickedCommand(string[] args)
        {
            try
            {
                var epochs = int.Parse(args[0]);
                var learningRate = double.Parse(args[1]);
                var regularization = double.Parse(args[2]);
                var momentum = double.Parse(args[3]);

                await TrainNetwork(epochs, learningRate, regularization, momentum);
            }
            catch (Exception ex)
            {
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
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

        private async Task TrainNetwork(int epochs, double learningRate, double regularization, double momentum)
        {
            if (_networksService.ActiveNetwork == null)
                return;

            var images = (await LoadImages(true)).Where(x => x.Category == Models.ImageCategory.Training);
            
            var inputs = images.Select(x => x.Pixels.Select(Convert.ToDouble).ToArray()).ToArray();
            var targets = CreateTargets(images.Select(x => x.Label).ToArray());

            await Task.Run(() =>
            {
                for (int epoch = 0; epoch < epochs; epoch++)
                {
                    _networksService.ActiveNetwork.Learn(inputs, targets, learningRate, regularization, momentum);
                }
            });

            await _networksService.SaveNetworkAsync(_networksService.ActiveNetwork);

            TrainingEnded?.Invoke(this, null);
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

        private double[][] CreateTargets(int[] labels)
        {
            int numLabels = labels.Length;
            double[][] targets = new double[numLabels][];

            for (int i = 0; i < numLabels; i++)
            {
                targets[i] = new double[10];
                targets[i][labels[i]] = 1.0;
            }

            return targets;
        }

        private async Task<List<Models.Image>> LoadImages(bool refresh = false)
        {
            var images = new List<Models.Image>();

            try
            {
                images = await _imagesService.GetImagesAsync(refresh);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex.ToString());
                await _notificationService.ShowAsync(ex.Message, InfoBarSeverity.Error);
            }

            return images;
        }
    }
}