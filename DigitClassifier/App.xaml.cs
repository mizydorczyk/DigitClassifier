using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DigitClassifier.Interfaces;
using DigitClassifier.Services;
using DigitClassifier.ViewModels;
using DigitClassifier.Views;

namespace DigitClassifier
{
    public partial class App : Application
    {
        public IHost Host { get; private set; }

        public static T GetService<T>() where T : class
        {
            if ((Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} needs to be registered.");
            }

            return service;
        }

        public static Window MainWindow { get; } = new MainWindow();
        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.
                CreateDefaultBuilder().
                UseContentRoot(AppContext.BaseDirectory).
                ConfigureServices((context, services) =>
                {
                    // services
                    services.AddSingleton<IActivationService, ActivationService>();

                    // views and view models
                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();
                }).
                Build();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            GetService<IActivationService>().Activate(args);
        }
    }
}
