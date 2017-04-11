using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.CustomTabs;
using Auth0.OidcClient;
using IdentityModel.OidcClient;

namespace XamarinAndroidTestApp
{
    [Activity(Label = "XamarinAndroidTestApp", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class MainActivity : Activity
    {
        private Auth0Client _client;
        private Button _loginButton;
        private TextView _userDetailsTextView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            _userDetailsTextView = FindViewById<TextView>(Resource.Id.UserDetailsTextView);

            _client = new Auth0Client(Resources.GetString(Resource.String.auth0_domain), Resources.GetString(Resource.String.auth0_client_id), this, scope: "openid name");

            _loginButton.Click += LoginButtonOnClick;
            _userDetailsTextView.Text = String.Empty;
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            var loginResult = await _client.LoginAsync();

            var sb = new StringBuilder();
            if (loginResult.IsError)
            {
                sb.AppendLine($"An error occurred during login: {loginResult.Error}");
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

            _userDetailsTextView.Text = sb.ToString();
        }
    }
}

