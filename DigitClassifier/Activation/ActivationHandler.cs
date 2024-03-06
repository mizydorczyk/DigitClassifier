namespace DigitClassifier.Activation;

public abstract class ActivationHandler<T> : IActivationHandler where T : class
{
    protected virtual bool CanHandleInternal(T args) => true;

    protected abstract Task HandleInternalAsync(T args);

    public bool CanHandle(object args) => args is T && CanHandleInternal((T)args!);

    public async Task HandleAsync(object args) => await HandleInternalAsync((T)args!);
}