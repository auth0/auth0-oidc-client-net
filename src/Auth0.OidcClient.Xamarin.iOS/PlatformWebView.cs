using System;
using System.Threading.Tasks;
using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using UIKit;

namespace Auth0.OidcClient
{
	public class PlatformWebView : SFSafariViewControllerDelegate, IBrowser
    {
        private SFAuthenticationSession _authSession;
        private SFSafariViewController _safari;

        public override void DidFinish(SFSafariViewController controller)
	    {
	        ActivityMediator.Instance.Send("UserCancel");
	    }

        public Task<BrowserResult> InvokeAsync(BrowserOptions options)
		{
			if (string.IsNullOrWhiteSpace(options.StartUrl))
			{
				throw new ArgumentException("Missing StartUrl", nameof(options));
			}

			if (string.IsNullOrWhiteSpace(options.EndUrl))
			{
				throw new ArgumentException("Missing EndUrl", nameof(options));
			}

			// must be able to wait for the authentication session to be finished to continue
			// with setting the task result
			var tcs = new TaskCompletionSource<BrowserResult>();

            // For iOS 11, we use the new SFAuthenticationSession
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
		    {
		        // create the authentication session
		        _authSession = new SFAuthenticationSession(
		            new NSUrl(options.StartUrl),
		            options.EndUrl,
		            (callbackUrl, error) =>
		            {
		                if (error != null)
		                {
		                    tcs.SetResult(new BrowserResult
		                    {
		                        ResultType = BrowserResultType.UserCancel,
		                        Error = error.ToString()
		                    });
		                }
		                else
		                {
		                    tcs.SetResult(new BrowserResult
		                    {
		                        ResultType = BrowserResultType.Success,
		                        Response = callbackUrl.AbsoluteString
		                    });
		                }
		            });

		        // launch authentication session
		        _authSession.Start();
		    }
            else // For pre-iOS 11, we use a normal SFSafariViewController
            {
		        // create Safari controller
		        _safari = new SFSafariViewController(new NSUrl(options.StartUrl))
		        {
		            Delegate = this
		        };

		        ActivityMediator.MessageReceivedEventHandler callback = null;
		        callback = async (response) =>
		        {
		            // remove handler
		            ActivityMediator.Instance.ActivityMessageReceived -= callback;

		            if (response == "UserCancel")
		            {
		                tcs.SetResult(new BrowserResult
		                {
		                    ResultType = BrowserResultType.UserCancel
		                });
		            }
		            else
		            {
		                // Close Safari
		                await _safari.DismissViewControllerAsync(true);

		                // set result
		                tcs.SetResult(new BrowserResult
		                {
		                    Response = response,
		                    ResultType = BrowserResultType.Success
		                });
		            }
		        };

		        // attach handler
		        ActivityMediator.Instance.ActivityMessageReceived += callback;

		        // https://forums.xamarin.com/discussion/24689/how-to-acces-the-current-view-uiviewcontroller-from-an-external-service
		        var window = UIApplication.SharedApplication.KeyWindow;
		        var vc = window.RootViewController;
		        while (vc.PresentedViewController != null)
		        {
		            vc = vc.PresentedViewController;
		        }

                // launch Safari
                vc.PresentViewController(_safari, true, null);
            }

		    // Result for this task will be set in the authentication session
            // completion handler
			return tcs.Task;
		}
	}
}
