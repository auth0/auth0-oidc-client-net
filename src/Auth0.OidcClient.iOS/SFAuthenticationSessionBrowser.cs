using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using System.Threading.Tasks;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the Browser interface using SFAuthenticationSession for support on iOS 11.
    /// </summary>
    public class SFAuthenticationSessionBrowser : IOSBrowserBase
    {
        protected override Task<BrowserResult> Launch(BrowserOptions options)
        {
            return Start(options);
        }

        internal static Task<BrowserResult> Start(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            SFAuthenticationSession sfWebAuthenticationSession = null;
            sfWebAuthenticationSession = new SFAuthenticationSession(
                new NSUrl(options.StartUrl),
                options.EndUrl,
                (callbackUrl, error) =>
                {
                    tcs.SetResult(CreateBrowserResult(callbackUrl, error));
                    sfWebAuthenticationSession.Dispose();
                });

            sfWebAuthenticationSession.Start();

            return tcs.Task;
        }

        private static BrowserResult CreateBrowserResult(NSUrl callbackUrl, NSError error)
        {
            if (error == null)
                return Success(callbackUrl.AbsoluteString);

            if (error.Code == (long)SFAuthenticationError.CanceledLogin)
                return Canceled();

            return UnknownError(error.ToString());
        }
    }
}
