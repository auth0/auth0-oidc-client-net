using Android.Content;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Implements the <see cref="IBrowser"/> interface using the best available option for the current Android version.
    /// </summary>
    public class AutoSelectBrowser : ChromeCustomTabsBrowser
    {
        /// <summary>
        /// Create a new instance of <see cref="AutoSelectBrowser"/> for a given <see cref="Context"/>.
        /// </summary>
        /// <param name="context"><see cref="Context"/> provided to any subsequent callback.</param>
        /// <param name="autoCloseBrowser">Whether to close the browser when returning to the main application or to keep it open.</param>
        public AutoSelectBrowser(Context context, bool autoCloseBrowser = false) : base(context)
        {
            this.AutoCloseBrowser = autoCloseBrowser;
        }

        /// <summary>
        /// Create a new instance of <see cref="AutoSelectBrowser"/>.
        /// </summary>
        internal AutoSelectBrowser(bool autoCloseBrowser = false) : this(null, autoCloseBrowser)
        {
        }
    }
}