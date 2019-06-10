using Android.App;
using Android.Content;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements browser integration using the a regular web browser.
    /// </summary>
    public class SystemBrowser : AndroidBrowserBase
    {
        protected override void OpenBrowser(Android.Net.Uri uri)
        {
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NoHistory)
                .AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}