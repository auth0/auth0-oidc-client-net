using System;

using UIKit;
using Auth0.OidcClient;
using IdentityModel.OidcClient;
using Foundation;

namespace XamariniOSTestApp
{
	public partial class MyViewController : UIViewController
	{
		private SafariServices.SFSafariViewController safari;
		private Auth0Client _client;
		private AuthorizeState _state;

		public MyViewController() : base("MyViewController", null)
		{
			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			LoginButton.TouchUpInside += LoginButton_TouchUpInside;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private async void LoginButton_TouchUpInside(object sender, EventArgs e)
		{
			_client = new Auth0Client("jerrie.auth0.com", "vV9twaySQzfGesS9Qs6gOgqDsYDdgoKE", scope: "openid name");

			_state = await _client.PrepareLoginAsync(null);

			AppDelegate.CallbackHandler = HandleCallback;
			safari = new SafariServices.SFSafariViewController(new NSUrl(_state.StartUrl));

			this.PresentViewController(safari, true, null);
		}

		private async void HandleCallback(string url)
		{
			await safari.DismissViewControllerAsync(true);

			var result = await _client.ProcessResponseAsync(url, _state);

			if (result.IsError)
			{
				
				return;
			}

			//var sb = new StringBuilder(128);
			//foreach (var claim in result.User.Claims)
			//{
			//	sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
			//}

			//sb.AppendFormat("\n{0}: {1}\n", "refresh token", result?.RefreshToken ?? "none");
			//sb.AppendFormat("\n{0}: {1}\n", "access token", result.AccessToken);

			//OutputTextView.Text = sb.ToString();

			//_apiClient = new HttpClient();
			//_apiClient.SetBearerToken(result.AccessToken);
			//_apiClient.BaseAddress = new Uri("https://api.identityserver.io");

			//CallApiButton.Enabled = true;
		}
	}
}

