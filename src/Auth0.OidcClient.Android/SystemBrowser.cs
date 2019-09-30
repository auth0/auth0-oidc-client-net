using Android.Content;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements browser integration using the a regular web browser.
    /// </summary>
    public class SystemBrowser : AndroidBrowserBase
    {
        public SystemBrowser(Context context = null)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OpenBrowser(Android.Net.Uri uri, Context context = null)
        {
            var intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NoHistory);

            if (IsNewTask)
                intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
        }
    }
}