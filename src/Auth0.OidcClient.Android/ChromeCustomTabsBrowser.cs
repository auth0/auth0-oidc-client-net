using Android.Content;
using Android.Support.CustomTabs;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements browser integration using Chrome Custom Tabs.
    /// </summary>
    public class ChromeCustomTabsBrowser : AndroidBrowserBase
    {
        /// <summary>
        /// Create a new instance of <see cref="ChromeCustomTabsBrowser"/> for a given <see cref="Context"/>.
        /// </summary>
        /// <param name="context"><see cref="Context"/> provided to any subsequent callback.</param>
        public ChromeCustomTabsBrowser(Context context = null)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OpenBrowser(Android.Net.Uri uri, Context context = null)
        {
            using (var builder = new CustomTabsIntent.Builder())
            using (var customTabsIntent = builder.Build())
            {
                customTabsIntent.Intent.AddFlags(ActivityFlags.NoHistory);
                if (IsNewTask)
                    customTabsIntent.Intent.AddFlags(ActivityFlags.NewTask);
                customTabsIntent.LaunchUrl(context, uri);
            }
        }
    }
}