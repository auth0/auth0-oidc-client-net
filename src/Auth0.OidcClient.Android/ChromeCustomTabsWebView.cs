using Android.App;
using Android.Content;
using Android.Support.CustomTabs;

namespace Auth0.OidcClient
{
    public class ChromeCustomTabsWebView : AndroidWebViewBase
    {
        protected override void LaunchBrowser(Android.Net.Uri uri)
        {
            var builder = new CustomTabsIntent.Builder();
            var customTabsIntent = builder.Build();
            customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);
            customTabsIntent.LaunchUrl(Application.Context, uri);
        }
    }
}