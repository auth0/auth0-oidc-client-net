using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using <see cref="SFSafariViewController"/> for support on iOS 10 and earlier.
    /// </summary>
    public class SFSafariViewControllerBrowser : IOSBrowserBase
    {
        /// <inheritdoc/>
        protected override Task<BrowserResult> Launch(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            return Start(options);
        }

        internal static Task<BrowserResult> Start(BrowserOptions options)
        {
            var tcs = new TaskCompletionSource<BrowserResult>();

            // Create Safari controller
            var safari = new SFSafariViewController(new NSUrl(options.StartUrl))
            {
                Delegate = new SafariViewControllerDelegate()
            };

            async void Callback(string response)
            {
                ActivityMediator.Instance.ActivityMessageReceived -= Callback;

                if (response == "UserCancel")
                {
                    tcs.SetResult(Canceled());
                }
                else
                {
                    await safari.DismissViewControllerAsync(true); // Close Safari
                    safari.Dispose();
                    tcs.SetResult(Success(response));
                }
            }

            ActivityMediator.Instance.ActivityMessageReceived += Callback;

            // Launch Safari
            FindRootController().PresentViewController(safari, true, null);

            return tcs.Task;
        }

        private static UIViewController FindRootController()
        {
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }

        class SafariViewControllerDelegate : SFSafariViewControllerDelegate
        {
            public override void DidFinish(SFSafariViewController controller)
            {
                ActivityMediator.Instance.Send("UserCancel");
            }
        }
    }
}
