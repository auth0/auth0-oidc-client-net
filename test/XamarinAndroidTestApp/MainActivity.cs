using System;
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
        //private AuthorizeState _state;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _client = new Auth0Client(Resources.GetString(Resource.String.auth0_domain), Resources.GetString(Resource.String.auth0_client_id), this, scope: "openid name");

            //string oauthCallbackData = Intent.GetStringExtra("OAuthCallbackData");
            //if (!string.IsNullOrEmpty(oauthCallbackData))
            //{
            //    await _client.ProcessResponseAsync(oauthCallbackData, _state);
            //}
            
            Button loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += LoginButtonOnClick;
        }

        private async void LoginButtonOnClick(object sender, EventArgs eventArgs)
        {
            var loginResult = await _client.LoginAsync();

            //_state = await _client.PrepareLoginAsync();

            //var customTabs = new CustomTabsActivityManager(this);
            //var builder = new CustomTabsIntent.Builder(customTabs.Session)
            //   .SetToolbarColor(Color.Argb(255, 52, 152, 219))
            //   .SetShowTitle(true)
            //   .EnableUrlBarHiding();

            //var customTabsIntent = builder.Build();
            //customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);

            //customTabsIntent.LaunchUrl(this, Android.Net.Uri.Parse(_state.StartUrl));
        }
    }
}

