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
    [Activity(Label = "XamarinAndroidTestApp", MainLauncher = true, Icon = "@drawable/icon",
        LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    //[Activity(Label = "Auth0 OIDC Callback Activity")]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "xamarinandroidtestapp.xamarinandroidtestapp",
        DataHost = "@string/auth0_domain",
        DataPathPrefix = "/android/xamarinandroidtestapp.xamarinandroidtestapp/callback")]
    public class MainActivity : Activity
    {
        private Auth0Client _client;
        private Button _loginButton;
        private TextView _userDetailsTextView;
        //private bool _authenticating = false;
        private AuthorizeState authorizeState;

        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var loginResult = await _client.ProcessResponseAsync(intent.DataString, authorizeState);

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
            /*
            // If we are receiving an intent and we are busy authenticating, then we need to close the authentication
            // loop by Send-ing the data string of the intent with the ActivityMediator. 
            // The internal process is that the DataString will contain a code which the Auth0 OIDC client will exchange 
            // for the tokens. It will also complete the task which was await-ed when you called LoginAsync() so your code
            // will resume from that point on
            if (_authenticating)
            {
                _authenticating = false;
                ActivityMediator.Instance.Send(intent.DataString);
            }
            */
        }

        protected override void OnResume()
        {
            base.OnResume();

            /*
            // If we are resuming and the _authenticating flag is still set, it means that the intent was not fired, so 
            // possible the user pressed the Back button. In this case we have to cancel the authentication process
            // by calling the Cancel() method on the ActivityMediator. This will also complete the task which was await-ed when 
            // you called LoginAsync() so your code will resume from that point on.
            if (_authenticating)
            {
                _authenticating = false;
                ActivityMediator.Instance.Cancel();
            }
            */
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            _userDetailsTextView = FindViewById<TextView>(Resource.Id.UserDetailsTextView);

            _client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = Resources.GetString(Resource.String.auth0_domain),
                ClientId = Resources.GetString(Resource.String.auth0_client_id),
                Activity = this
            });

            _loginButton.Click += LoginButtonOnClick;
            _userDetailsTextView.Text = String.Empty;
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            _userDetailsTextView.Text = "";

            //// Set a flag we're authenticating
            //_authenticating = true;

            //// Call the login method
            //var loginResult = await _client.LoginAsync();

            authorizeState = await _client.PrepareLoginAsync();

            var customTabs = new CustomTabsActivityManager(this);

            // build custom tab
            var builder = new CustomTabsIntent.Builder(customTabs.Session)
               .SetToolbarColor(Color.Argb(255, 52, 152, 219))
               .SetShowTitle(true)
               .EnableUrlBarHiding();

            var customTabsIntent = builder.Build();
            customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

            customTabsIntent.LaunchUrl(this, Android.Net.Uri.Parse(authorizeState.StartUrl));
        }
    }
}