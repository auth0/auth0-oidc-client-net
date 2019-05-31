using Android.App;
using Android.Content;

namespace Auth0.OidcClient
{
    public class SystemBrowserWebView : AndroidWebViewBase
    {
        protected override void LaunchBrowser(Android.Net.Uri uri)
        {
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NoHistory)
                .AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}