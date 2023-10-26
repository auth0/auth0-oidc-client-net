using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Auth0.OidcClient.Platforms.Windows
{
    public interface IActivator
    {
        bool RedirectActivationChecked { get; }
        bool CheckRedirectionActivation();
    }

    /// <summary>
    /// Activator class used to enable protocol activation check and redirects activation to the correct application instance
    /// </summary>
    public sealed class Activator : IActivator
    {
        private readonly IAppInstanceProxy _appInstanceProxy;

        public static readonly Activator Default = new Activator(new AppInstanceProxy());

        internal Activator(IAppInstanceProxy appInstanceProxy)
        {
            _appInstanceProxy = appInstanceProxy;
        }

        /// <summary>
        /// Boolean indication the redirect activation was checked
        /// </summary>
        public bool RedirectActivationChecked { get; internal set; }

        /// <summary>
        /// Performs a protocol activation check and redirects activation to the correct application instance.
        /// </summary>
        public bool CheckRedirectionActivation()
        {
            var activatedEventArgs = _appInstanceProxy.GetCurrentActivatedEventArgs();

            RedirectActivationChecked = true;

            if (activatedEventArgs is null || activatedEventArgs.Kind != ExtendedActivationKind.Protocol || activatedEventArgs.Data is not IProtocolActivatedEventArgs protocolArgs)
            {
                return false;
            }

            var ctx = RedirectionContextManager.GetRedirectionContext(protocolArgs);

            if (ctx is not null && ctx.AppInstanceKey is not null && ctx.TaskId is not null)
            {
                return _appInstanceProxy.RedirectActivationToAsync(ctx.AppInstanceKey, activatedEventArgs);
            }
            else
            {
                _appInstanceProxy.FindOrRegisterForKey();
            }
            return false;
        }

    }
}