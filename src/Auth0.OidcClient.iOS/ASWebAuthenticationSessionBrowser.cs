using AuthenticationServices;
using Foundation;
using IdentityModel.OidcClient.Browser;
using System.Threading.Tasks;
using UIKit;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using <see cref="ASWebAuthenticationSession"/> for support on iOS 12+.
    /// </summary>
    public class ASWebAuthenticationSessionBrowser : IOSBrowserBase
    {
        /// <inheritdoc/>
        protected override Task<BrowserResult> Launch(BrowserOptions options)
        {
            return Start(options);
        }

        internal static Task<BrowserResult> Start(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            ASWebAuthenticationSession asWebAuthenticationSession = null;
            asWebAuthenticationSession = new ASWebAuthenticationSession(
                new NSUrl(options.StartUrl),
                options.EndUrl,
                (callbackUrl, error) =>
                {
                    tcs.SetResult(CreateBrowserResult(callbackUrl, error));
                    asWebAuthenticationSession.Dispose();
                });

            // iOS 13 requires the PresentationContextProvider set
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
                asWebAuthenticationSession.PresentationContextProvider = new PresentationContextProviderToSharedKeyWindow();

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
