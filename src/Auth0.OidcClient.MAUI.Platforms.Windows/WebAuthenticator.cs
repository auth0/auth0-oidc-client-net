using Microsoft.Windows.AppLifecycle;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Activation;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Auth0.OidcClient.Platforms.Windows
{
    public sealed class WebAuthenticator
    {
        private readonly IHelpers _helpers;
        private readonly IAppInstanceProxy _appInstanceProxy;
        private readonly ITasksManager _tasksManager;
        private readonly IActivator _activator;

        public static readonly WebAuthenticator Default = new WebAuthenticator(new AppInstanceProxy(), new Helpers(), TasksManager.Default, Activator.Default);

        internal WebAuthenticator(IAppInstanceProxy appInstanceProxy, IHelpers helpers, ITasksManager tasksManager, IActivator activator)
        {
            _helpers = helpers;
            _appInstanceProxy = appInstanceProxy;
            _tasksManager = tasksManager;
            _activator = activator;
            appInstanceProxy.Activated += CurrentAppInstance_Activated;
        }

        /// <summary>
        /// When the current application is activated, we want to see if there is a corresponding task that needs to be resumed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The <see cref="AppActivationArguments"/> event.</param>
        private void CurrentAppInstance_Activated(object? sender, IAppActivationArguments e)
        {
            if (e.Kind == ExtendedActivationKind.Protocol)
            {
                if (e.Data is IProtocolActivatedEventArgs protocolArgs)
                {
                    var ctx = RedirectionContextManager.GetRedirectionContext(protocolArgs);

                    if (ctx is not null && ctx.TaskId is not null)
                    {
                        _tasksManager.ResumeTask(protocolArgs.Uri, ctx.TaskId);
                    }
                }
            }
        }

        /// <summary>
        /// Begin an authentication flow by navigating to the specified url and waiting for a callback/redirect to the callbackUrl scheme.
        /// </summary>
        /// <param name="authorizeUri">Url to navigate to, beginning the authentication flow.</param>
        /// <param name="callbackUri">Expected callback url that the navigation flow will eventually redirect to.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a result parsed out from the callback url.</returns>
        /// <remarks>Prior to calling this, a call to <see cref="Activator.CheckRedirectionActivation()"/> must be made during application startup.</remarks>
        /// <seealso cref="Activator.CheckRedirectionActivation()"/>
        public async Task<WebAuthenticatorResult> AuthenticateAsync(Uri authorizeUri, Uri callbackUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_activator.RedirectActivationChecked)
            {
                throw new InvalidOperationException("The redirection check on app activation was not detected. Please make sure a call to Activator.CheckRedirectionActivation was made during App creation.");
            }
            if (!_helpers.IsAppPackaged)
            {
                throw new InvalidOperationException("The WebAuthenticator requires a packaged app with an AppxManifest.");
            }
            if (!_helpers.IsUriProtocolDeclared(callbackUri.Scheme))
            {
                throw new InvalidOperationException($"The URI Scheme {callbackUri.Scheme} is not declared in AppxManifest.xml.");
            }

            var redirectContext = RedirectionContext.New(_appInstanceProxy);

            authorizeUri = StateModifier.WrapStateWithRedirectionContext(authorizeUri, redirectContext);

            var tcs = new TaskCompletionSource<Uri>();

            if (cancellationToken.CanBeCanceled)
            {
                // When the cancellationToken is used to trigger the cancellation
                // we need to cancel the TaskCompletionSource and remove the taskid from the tasks.
                cancellationToken.Register(() =>
                {
                    tcs.TrySetCanceled();
                    _tasksManager.Remove(redirectContext.TaskId);
                });

                cancellationToken.ThrowIfCancellationRequested();
            }

            var newUri = authorizeUri.AbsolutePath == "/v2/logout" ? StateModifier.MoveStateToReturnTo(authorizeUri) : authorizeUri;

            _helpers.OpenBrowser(newUri);

            _tasksManager.Add(redirectContext.TaskId, tcs);
            var uri = await tcs.Task.ConfigureAwait(false);
            return new WebAuthenticatorResult(StateModifier.UnwrapRedirectionContextFromState(uri));
        }
    }
}