using Android.App;
using Android.Content;
using Android.Support.CustomTabs;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements browser integration using Chrome Custom Tabs.
    /// </summary>
    public class ChromeCustomTabsBrowser : AndroidBrowserBase
    {
        protected override void OpenBrowser(Android.Net.Uri uri)
        {
            using (var builder = new CustomTabsIntent.Builder())
            using (var customTabsIntent = builder.Build())
            {
                customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);
                customTabsIntent.LaunchUrl(Application.Context, uri);
            }
        }
    }
}