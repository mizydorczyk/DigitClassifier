using Microsoft.UI.Xaml.Controls;

namespace DigitClassifier.Interfaces
{
    public interface INotificationService
    {
        void Initialize(InfoBar infoBar);
        Task ShowAsync(string message, InfoBarSeverity severity, int persistanceTime = 5000);
    }
}