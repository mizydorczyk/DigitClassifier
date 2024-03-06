using DigitClassifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; private set; }

        public SettingsPage()
        {
            ViewModel = App.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}