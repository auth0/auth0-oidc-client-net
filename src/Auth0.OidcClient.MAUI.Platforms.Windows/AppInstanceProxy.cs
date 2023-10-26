using System.Diagnostics.CodeAnalysis;
using Microsoft.Windows.AppLifecycle;

namespace Auth0.OidcClient.Platforms.Windows;

internal interface IAppInstanceProxy
{
    event EventHandler<IAppActivationArguments> Activated;
    string GetCurrentAppKey();
    Microsoft.Windows.AppLifecycle.AppActivationArguments GetCurrentActivatedEventArgs();

    bool RedirectActivationToAsync(string key,
        Microsoft.Windows.AppLifecycle.AppActivationArguments activatedEventArgs);

    void FindOrRegisterForKey();
}

/// <summary>
/// Excludes from Code Coverage because of the integration with AppInstance.GetCurrent()
/// </summary>
[ExcludeFromCodeCoverage]
internal class AppInstanceProxy : IAppInstanceProxy
{
    public AppInstanceProxy()
    {
        AppInstance.GetCurrent().Activated += OnActivated;
    }

    public event EventHandler<IAppActivationArguments> Activated;

    protected virtual void OnActivated(object? sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
    {
        Activated?.Invoke(this, new AppActivationArguments
        {
            Kind = e.Kind,
            Data = e.Data
        });
    }

    /// <summary>
    /// Get the current application key.
    /// </summary>
    /// <remarks>
    /// Proxy call to AppInstance.GetCurrent().Key.
    /// Used because AppInstance is complicated to use in tests.
    /// </remarks>
    /// <returns>The key for the current application.</returns>
    public virtual string GetCurrentAppKey()
    {
        return AppInstance.GetCurrent().Key;
    }

    /// <summary>
    /// Get the current application <see cref="AppActivationArguments"/>
    /// </summary>
    /// <remarks>
    /// Proxy call to AppInstance.GetCurrent().GetActivatedEventArgs().
    /// Used because AppInstance is complicated to use in tests.
    /// </remarks>
    /// <returns>Null if no current application instance is found, or the corresponding <see cref="AppActivationArguments"/>.</returns>
    public virtual Microsoft.Windows.AppLifecycle.AppActivationArguments GetCurrentActivatedEventArgs()
    {
        return AppInstance.GetCurrent()?.GetActivatedEventArgs();
    }

    /// <summary>
    /// Redirect the activation to the correct application instance and kill the current process.
    /// </summary>
    /// <param name="key">Key of the application to activated</param>
    /// <param name="activatedEventArgs"><see cref="AppActivationArguments"/> to pass to the application.</param>
    /// <returns>Boolean indicating an application instance was activated.</returns>
    public virtual bool RedirectActivationToAsync(string key, Microsoft.Windows.AppLifecycle.AppActivationArguments activatedEventArgs)
    {
        var instance = AppInstance.GetInstances().FirstOrDefault(i => i.Key == key);

        if (instance is not null && !instance.IsCurrent)
        {
            instance.RedirectActivationToAsync(activatedEventArgs).AsTask().Wait();

            System.Diagnostics.Process.GetCurrentProcess().Kill();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Registers the current application using a new key.
    /// </summary>
    public virtual void FindOrRegisterForKey()
    {
        var instance = AppInstance.GetCurrent();

        if (string.IsNullOrEmpty(instance.Key))
        {
            AppInstance.FindOrRegisterForKey(Guid.NewGuid().ToString());
        }
    }
}