using System.Diagnostics.CodeAnalysis;
using Microsoft.Windows.AppLifecycle;

namespace Auth0.OidcClient.Platforms.Windows;

internal interface IAppInstanceProxy
{
    event EventHandler<IAppActivationArguments> Activated;
    string GetCurrentAppKey();
}

internal interface IAppActivationArguments
{
    ExtendedActivationKind Kind { get; set; }
    object Data { get; set; }
}

internal class AppActivationArguments : IAppActivationArguments
{
    public ExtendedActivationKind Kind { get; set; }
    public object Data { get; set; }
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

    public virtual string GetCurrentAppKey()
    {
        return AppInstance.GetCurrent().Key;
    }
}