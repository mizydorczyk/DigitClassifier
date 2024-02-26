using DigitClassifier.Interfaces;
using DigitClassifier.ViewModels;
using DigitClassifier.Views;
using Microsoft.UI.Xaml;

namespace DigitClassifier.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        _navigationService.NavigateTo(typeof(ImagesViewModel).FullName!, args.Arguments);

        await Task.CompletedTask;
    }
}
