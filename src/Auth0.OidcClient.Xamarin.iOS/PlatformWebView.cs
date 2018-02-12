using System;
using System.Threading.Tasks;
using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using UIKit;

namespace Auth0.OidcClient
{
	public class PlatformWebView : IBrowser
	{
        private SFAuthenticationSession _authSession;

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

			// Result for this task will be set in the authentication session
            // completion handler
			return tcs.Task;
		}
	}
}
