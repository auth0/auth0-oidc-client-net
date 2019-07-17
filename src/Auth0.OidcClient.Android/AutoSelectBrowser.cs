using Android.Content;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implement recommended browser integration for this platform.
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