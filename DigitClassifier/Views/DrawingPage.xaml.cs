using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class DrawingPage : Page
    {
        public DrawingViewModel ViewModel { get; private set; }
        public DrawingPage()
        {
            ViewModel = App.GetService<DrawingViewModel>(); ;
            InitializeComponent();
        }
    }
}
