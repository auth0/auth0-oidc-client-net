using System;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provided for backward compatibility with earlier versions of Auth0.OidcClient.WinForms by implementing
    /// <see cref="WebBrowserBrowser"/>.
    /// </summary>
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebBrowserBrowser if you need custom form handling.")]
    public class PlatformWebView : WebBrowserBrowser
    {
        /// <inheritdoc/>
        public PlatformWebView(Func<Form> formFactory)
            : base(formFactory)
        {
        }

        /// <inheritdoc/>
        public PlatformWebView(string title = "Authenticating...", int width = 1024, int height = 768)
            : base(title, width, height)
        {
        }
    }
}