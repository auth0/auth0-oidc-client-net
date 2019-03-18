using System;
using System.Threading.Tasks;
using AuthenticationServices;
using Foundation;
using IdentityModel.OidcClient.Browser;
using SafariServices;
using UIKit;

namespace Auth0.OidcClient
{
	public class PlatformWebView : SFSafariViewControllerDelegate, IBrowser
	{
		private SFAuthenticationSession _sfAuthenticationSession;
		private ASWebAuthenticationSession _asWebAuthenticationSession;
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

			// For iOS 12, we use ASWebAuthenticationSession
			if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
			{
				// create the authentication session
				_asWebAuthenticationSession = new ASWebAuthenticationSession(
					new NSUrl(options.StartUrl),
					options.EndUrl,
					(callbackUrl, error) =>
					{
						var browserResult = new BrowserResult();

						if (error != null)
						{
							if (error.Code == (long)ASWebAuthenticationSessionErrorCode.CanceledLogin)
								browserResult.ResultType = BrowserResultType.UserCancel;
							else
								browserResult.ResultType = BrowserResultType.UnknownError;

							browserResult.Error = error.ToString();

							tcs.SetResult(browserResult);
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
				_asWebAuthenticationSession.Start();
			}
			// For iOS 11, we use SFAuthenticationSession
			else if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
			{
				// create the authentication session
				_sfAuthenticationSession = new SFAuthenticationSession(
					new NSUrl(options.StartUrl),
					options.EndUrl,
					(callbackUrl, error) =>
					{
						var browserResult = new BrowserResult();

						if (error != null)
						{
							if (error.Code == (long)SFAuthenticationError.CanceledLogin)
								browserResult.ResultType = BrowserResultType.UserCancel;
							else
								browserResult.ResultType = BrowserResultType.UnknownError;

							browserResult.Error = error.ToString();

							tcs.SetResult(browserResult);
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
				_sfAuthenticationSession.Start();
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
