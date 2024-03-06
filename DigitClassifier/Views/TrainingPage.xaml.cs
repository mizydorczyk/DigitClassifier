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
        }
    }
}