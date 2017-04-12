using System;

using UIKit;
using Auth0.OidcClient;
using IdentityModel.OidcClient;
using Foundation;
using System.Text;

namespace XamariniOSTestApp
{
	public partial class MyViewController : UIViewController
	{
		private Auth0Client _client;

		public MyViewController() : base("MyViewController", null)
		{

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			UserDetailsTextView.Text = String.Empty;

			LoginButton.TouchUpInside += LoginButton_TouchUpInside;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		private async void LoginButton_TouchUpInside(object sender, EventArgs e)
		{
		    _client = new Auth0Client(new Auth0ClientOptions
		    {
		        Domain = "jerrie.auth0.com",
		        ClientId = "vV9twaySQzfGesS9Qs6gOgqDsYDdgoKE",
		        Controller = this
		    });

			var loginResult = await _client.LoginAsync(null);

            var sb = new StringBuilder();

            if (loginResult.IsError)
            {
                sb.AppendLine("An error occurred during login:");
                sb.AppendLine(loginResult.Error);
            }
            else
            {
                sb.AppendLine($"ID Token: {loginResult.IdentityToken}");
                sb.AppendLine($"Access Token: {loginResult.AccessToken}");
                sb.AppendLine($"Refresh Token: {loginResult.RefreshToken}");
                sb.AppendLine();
                sb.AppendLine("-- Claims --");
                foreach (var claim in loginResult.User.Claims)
                {
                    sb.AppendLine($"{claim.Type} = {claim.Value}");
                }
            }

            UserDetailsTextView.Text = sb.ToString();
		}
	}
}

