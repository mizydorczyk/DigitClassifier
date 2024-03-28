using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class TrainingPage : Page
    {
        public TrainingViewModel ViewModel { get; private set; }

        public TrainingPage()
        {
            ViewModel = App.GetService<TrainingViewModel>();
            InitializeComponent();

            ViewModel.TrainingEnded += ViewModel_TrainingEnded;
        }

        private void ViewModel_TrainingEnded(object? sender, EventArgs e)
        {
            TrainButton.IsEnabled = true;
        }

        private void TrainButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            TrainButton.IsEnabled = false;
            ViewModel.TrainClickedCommand.Execute(new string[] { Epochs.Text, LearningRate.Text, Regularization.Text, Momentum.Text });
        }
    }
}