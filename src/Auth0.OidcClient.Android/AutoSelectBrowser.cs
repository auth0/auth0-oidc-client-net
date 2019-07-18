using Android.Content;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the best available option for the current Android version.
    /// </summary>
    public class AutoSelectBrowser : ChromeCustomTabsBrowser
    {
        public AutoSelectBrowser(Context context) : base(context)
        {
        }

        internal AutoSelectBrowser() : base(null)
        {
        }
    }
}