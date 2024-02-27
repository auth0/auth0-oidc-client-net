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
        public AutoSelectBrowser(Context context) : base(context)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="AutoSelectBrowser"/>.
        /// </summary>
        internal AutoSelectBrowser() : base(null)
        {
        }
    }
}