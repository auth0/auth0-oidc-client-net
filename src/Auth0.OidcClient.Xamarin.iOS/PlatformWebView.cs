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
		private SafariServices.SFSafariViewController _safari;
		private readonly UIViewController _controller;

		public PlatformWebView(UIViewController controller)
		{
			_controller = controller;
		}

        public override void DidFinish(SFSafariViewController controller)
        {
            ActivityMediator.Instance.Send("Cancelled");
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

			// must be able to wait for the intent to be finished to continue
			// with setting the task result
			var tcs = new TaskCompletionSource<BrowserResult>();

			// create Safari controller
			_safari = new SafariServices.SFSafariViewController(new NSUrl(options.StartUrl));
            _safari.Delegate = this;

			ActivityMediator.MessageReceivedEventHandler callback = null;
			callback = async (response) =>
			{
				// remove handler
				ActivityMediator.Instance.ActivityMessageReceived -= callback;

                if (response == "Cancelled")
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

			// launch Safari
			_controller.PresentViewController(_safari, true, null);

			// need an intent to be triggered when browsing to the "io.identitymodel.native://callback"
			// scheme/URI => CallbackInterceptorActivity
			return tcs.Task;
		}
	}
}
