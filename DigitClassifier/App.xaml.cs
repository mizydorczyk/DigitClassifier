using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DigitClassifier.Interfaces;
using DigitClassifier.Services;
using DigitClassifier.ViewModels;
using DigitClassifier.Views;
using WinUIEx;
using Microsoft.UI.Xaml;
using DigitClassifier.Activation;
using DigitClassifier.Models;
using Serilog;

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

        public static WindowEx MainWindow { get; } = new MainWindow();

        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .UseSerilog((hostingContext, services, loggerConfiguration) =>
                {
                    var path = Path.Combine(Path.GetTempPath(), $"DigitClassfier_{DateTime.UtcNow.ToString("ddMMyyyy_HHmmss")}.log");
                    loggerConfiguration.WriteTo.File(path);
                })
                .ConfigureServices((context, services) =>
                {
                    // activation handlers
                    services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                    // services
                    services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
                    services.AddSingleton<IActivationService, ActivationService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IPageService, PageService>();
                    services.AddSingleton<IImagesService, ImagesService>();

                    services.AddTransient<INavigationViewService, NavigationViewService>();

                    // views and view models
                    services.AddTransient<ShellPage>();
                    services.AddTransient<ShellViewModel>();

                    services.AddTransient<DrawingPage>();
                    services.AddTransient<DrawingViewModel>();

                    services.AddTransient<NetworksPage>();
                    services.AddTransient<NetworksViewModel>();

                    services.AddTransient<ImagesPage>();
                    services.AddTransient<ImagesViewModel>();

                    services.AddTransient<TrainingPage>();
                    services.AddTransient<TrainingViewModel>();

                    services.AddTransient<SettingsPage>();
                    services.AddTransient<SettingsViewModel>();

                    // configuration
                    services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
                }).Build();

            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Log.Logger.Fatal(e.Exception.ToString());
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            await GetService<IActivationService>().ActivateAsync(args);
        }
    }
}