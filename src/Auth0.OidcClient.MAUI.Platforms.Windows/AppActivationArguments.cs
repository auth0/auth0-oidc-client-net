using Microsoft.Windows.AppLifecycle;

namespace Auth0.OidcClient.Platforms.Windows;

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