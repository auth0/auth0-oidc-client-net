using IdentityModel.OidcClient.Browser;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the best available option for the current iOS version.
    /// </summary>
    public class AutoSelectBrowser : IOSBrowserBase
    {
        /// <inheritdoc/>
        protected override Task<BrowserResult> Launch(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            // For iOS 12+ use ASWebAuthenticationSession
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return ASWebAuthenticationSessionBrowser.Start(options);

            // For iOS 11 use SFAuthenticationSession
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                return SFAuthenticationSessionBrowser.Start(options);

            // For iOS 10 and earlier use SFSafariViewController
            return SFSafariViewControllerBrowser.Start(options);
        }
    }
}
