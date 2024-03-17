using DigitClassifier.Interfaces;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitClassifier.Services
{
    public class NotificationService : INotificationService
    {
        private const int _animationTimeSpan = 500;

        private InfoBar? _infoBar;
        private CancellationTokenSource? _cancellationTokenSource;

        public void Initialize(InfoBar infoBar)
        {
            _infoBar = infoBar;
        }

        public async Task ShowAsync(string message, InfoBarSeverity severity, int persistanceTime = 5000)
        {
            if (_infoBar == null)
                return;

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            try
            {
                _infoBar.Message = message;
                _infoBar.Severity = severity;
                _infoBar.IsOpen = true;

                var animation = new DoubleAnimation
                {
                    Duration = TimeSpan.FromMilliseconds(_animationTimeSpan),
                    From = 0.0,
                    To = 1.0,
                    EasingFunction = new CubicEase()
                };

                var storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, _infoBar);
                Storyboard.SetTargetProperty(animation, "Opacity");
                storyboard.Begin();

                await Task.Delay(_animationTimeSpan, cancellationToken);
                await Task.Delay(persistanceTime, cancellationToken);
                await Hide(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task Hide(CancellationToken cancellationToken)
        {
            if (_infoBar == null)
                return;

            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(_animationTimeSpan),
                From = 1.0,
                To = 0.0,
                EasingFunction = new CubicEase()
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, _infoBar);
            Storyboard.SetTargetProperty(animation, "Opacity");
            storyboard.Begin();

            try
            {
                await Task.Delay(_animationTimeSpan, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            _infoBar.IsOpen = false;
        }
    }
}