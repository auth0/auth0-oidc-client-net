using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using Auth0.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Text;

namespace AndroidTestApp
{
    [Activity(Label = "Android OIDC Test", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "androidtestapp.androidtestapp",
        DataHost = "@string/auth0_domain",
        DataPathPrefix = "/android/androidtestapp.androidtestapp/callback")]
    public class MainActivity : Activity
    {
        private Auth0Client _client;
        private TextView _userDetailsTextView;

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            ActivityMediator.Instance.Send(intent.DataString);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var clientOptions = new Auth0ClientOptions
            {
                Domain = Resources.GetString(Resource.String.auth0_domain),
                ClientId = Resources.GetString(Resource.String.auth0_client_id)
            };
            _client = new Auth0Client(clientOptions);

            SetContentView(Resource.Layout.Main);
            FindViewById<Button>(Resource.Id.LoginButton).Click += LoginButtonOnClick;
            FindViewById<Button>(Resource.Id.LogoutButton).Click += LogoutButtonOnClick;

            _userDetailsTextView = FindViewById<TextView>(Resource.Id.UserDetailsTextView);
            _userDetailsTextView.MovementMethod = new ScrollingMovementMethod();

            _userDetailsTextView.Text = $"App ID is {clientOptions.ClientId}\n" +
                $"Ensure Allowed Callback URLs includes\n\n{clientOptions.RedirectUri}\n\n" +
                $"and Allowed Logout URLs includes\n\n{clientOptions.PostLogoutRedirectUri}";
        }

        private async void LogoutButtonOnClick(object sender, EventArgs e)
        {
            _userDetailsTextView.Text = "Starting Logout process...";
            var logoutResult = await _client.LogoutAsync();
            _userDetailsTextView.Text = logoutResult == BrowserResultType.Success
                ? "Logout completed successfully"
                : $"Logout failed - ${logoutResult}";
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _userDetailsTextView.Text = "Starting Login process...";
            var loginResult = await _client.LoginAsync();
            _userDetailsTextView.Text = BuildLoginResultMessage(loginResult);
        }

        private static string BuildLoginResultMessage(IdentityModel.OidcClient.LoginResult loginResult)
        {
            if (loginResult.IsError)
                return $"Login failed - {loginResult.Error}";

            var sb = new StringBuilder();
            sb.AppendLine("-- Claims --");
            foreach (var claim in loginResult.User.Claims)
                sb.AppendLine($"{claim.Type} = {claim.Value}");

            sb.AppendLine();
            sb.AppendLine($"ID Token: {loginResult.IdentityToken}");
            sb.AppendLine($"Access Token: {loginResult.AccessToken}");
            sb.AppendLine($"Refresh Token: {loginResult.RefreshToken}");

            return sb.ToString();
        }
    }
}