using AuthenticationServices;
using Foundation;
using IdentityModel.OidcClient.Browser;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using <see cref="ASWebAuthenticationSession"/> for support on iOS 12+.
    /// </summary>
    public class ASWebAuthenticationSessionBrowser : IOSBrowserBase
    {
        /// <summary>
        /// Configuration for the ASWebAuthenticationSession.
        /// </summary>
        public ASWebAuthenticationSessionOptions SessionOptions { get; }

        /// <summary>
        /// Creates a new instance of the ASWebAuthenticationSession Browser.
        /// </summary>
        /// <param name="sessionOptions">The <see cref="ASWebAuthenticationSessionOptions"/> specifying the configuration for the ASWebAuthenticationSession.</param>
        /// <example>
        /// If any custom browser configuration is needed (e.g. using <see cref="ASWebAuthenticationSessionOptions.PrefersEphemeralWebBrowserSession"/>), 
        /// a new browser instance should be instantiated and passed to <see cref="Auth0ClientOptions.Browser"/>.
        /// <code>
        /// var client = new Auth0Client(new Auth0ClientOptions
        /// {
        ///   Domain = "YOUR_AUTH0_DOMAIN",
        ///   ClientId = "YOUR_AUTH0_CLIENT_ID",
        ///   Browser = new ASWebAuthenticationSessionBrowser(
        ///     new ASWebAuthenticationSessionOptions
        ///     {
        ///       PrefersEphemeralWebBrowserSession = true
        ///     }
        ///   )
        /// });
        /// </code>
        /// </example>
        public ASWebAuthenticationSessionBrowser(ASWebAuthenticationSessionOptions sessionOptions = null)
        {
            SessionOptions = sessionOptions;
        }

        /// <inheritdoc/>
        protected override Task<BrowserResult> Launch(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            return Start(options, SessionOptions);
        }

        internal static Task<BrowserResult> Start(BrowserOptions options, ASWebAuthenticationSessionOptions sessionOptions = null)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            ASWebAuthenticationSession asWebAuthenticationSession = null;
            asWebAuthenticationSession = new ASWebAuthenticationSession(
                new NSUrl(options.StartUrl),
                new NSUrl(options.EndUrl).Scheme,
                (callbackUrl, error) =>
                {
                    tcs.SetResult(CreateBrowserResult(callbackUrl, error));
                    asWebAuthenticationSession.Dispose();
                });

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                // iOS 13 requires the PresentationContextProvider set
                asWebAuthenticationSession.PresentationContextProvider = new PresentationContextProviderToSharedKeyWindow();
                // PrefersEphemeralWebBrowserSession is only available on iOS 13 and up.
                asWebAuthenticationSession.PrefersEphemeralWebBrowserSession = sessionOptions != null ? sessionOptions.PrefersEphemeralWebBrowserSession : false;
            }

            asWebAuthenticationSession.Start();

            return tcs.Task;
        }

        class PresentationContextProviderToSharedKeyWindow : NSObject, IASWebAuthenticationPresentationContextProviding
        {
            public UIWindow GetPresentationAnchor(ASWebAuthenticationSession session)
            {
                return UIApplication.SharedApplication.KeyWindow;
            }
        }

        private static BrowserResult CreateBrowserResult(NSUrl callbackUrl, NSError error)
        {
            if (error == null)
                return Success(callbackUrl.AbsoluteString);

            if (error.Code == (long)ASWebAuthenticationSessionErrorCode.CanceledLogin)
                return Canceled();

            return UnknownError(error.ToString());
        }
    }
}
