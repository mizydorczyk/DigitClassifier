using DigitClassifier.Interfaces;
using DigitClassifier.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Services
{
    public class ActivationService : IActivationService
    {
        private UIElement? _shell = null;

        public void Activate(object activationArgs)
        {
            if (App.MainWindow.Content == null)
            {
                _shell = App.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
            }

            App.MainWindow.Activate();
        }
    }
}
