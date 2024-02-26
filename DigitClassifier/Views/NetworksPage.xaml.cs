using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class NetworksPage : Page
    {
        public NetworksViewModel ViewModel { get; private set; }
        public NetworksPage()
        {
            ViewModel = App.GetService<NetworksViewModel>(); ;
            InitializeComponent();
        }
    }
}
