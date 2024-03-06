using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class ImagesPage : Page
    {
        public ImagesViewModel ViewModel { get; private set; }

        public ImagesPage()
        {
            ViewModel = App.GetService<ImagesViewModel>();
            InitializeComponent();
        }
    }
}