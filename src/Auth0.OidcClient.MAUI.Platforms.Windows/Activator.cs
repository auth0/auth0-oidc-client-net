using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Auth0.OidcClient.Platforms.Windows
{
    public sealed class Activator
    {
        internal static bool RedirectActivationCheck;

        /// <summary>
        /// Performs an protocol activation check and redirects activation to the correct application instance.
        /// </summary>
        public static bool CheckRedirectionActivation()
        {
            var activatedEventArgs = AppInstance.GetCurrent()?.GetActivatedEventArgs();

            RedirectActivationCheck = true;

            if (activatedEventArgs is null || activatedEventArgs.Kind != ExtendedActivationKind.Protocol || activatedEventArgs.Data is not IProtocolActivatedEventArgs protocolArgs)
            {
                return false;
            }

            var ctx = RedirectionContextManager.GetRedirectionContext(activatedEventArgs.Data as IProtocolActivatedEventArgs);

            if (ctx is not null && ctx.AppInstanceKey is not null && ctx.TaskId is not null)
            {
                var instance = AppInstance.GetInstances().FirstOrDefault(i => i.Key == ctx.AppInstanceKey);

                if (instance is not null && !instance.IsCurrent)
                {
                    instance.RedirectActivationToAsync(activatedEventArgs).AsTask().Wait();

                    System.Diagnostics.Process.GetCurrentProcess().Kill();

                    return true;
                }
            }
            else
            {
                var instance = AppInstance.GetCurrent();

                if (string.IsNullOrEmpty(instance.Key))
                {
                    AppInstance.FindOrRegisterForKey(Guid.NewGuid().ToString());
                }
            }
            return false;
        }

    }
}