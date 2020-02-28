using Android.Content;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the best available option for the current Android version.
    /// </summary>
    public class SystemBrowser : AndroidBrowserBase
    {
        /// <summary>
        /// Create a new instance of <see cref="SystemBrowser"/> for a given <see cref="Context"/>.
        /// </summary>
        /// <param name="context"><see cref="Context"/> provided to any subsequent callback.</param>
        public SystemBrowser(Context context = null)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override void OpenBrowser(Android.Net.Uri uri, Context context = null)
        {
            var intent = new Intent(Intent.ActionView, uri);

            if (IsNewTask)
                intent.AddFlags(ActivityFlags.NewTask);

            context.StartActivity(intent);
        }
    }
}