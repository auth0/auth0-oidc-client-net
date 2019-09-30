using System;
using System.Windows;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provided for backward compatibility with earlier versions of Auth0.OidcClient.WPF by implementing
    /// <see cref="WebBrowserBrowser"/>.
    /// </summary>
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebBrowserBrowser if you need custom window handling.")]
    public class PlatformWebView : WebBrowserBrowser
    {
        /// <inheritdoc/>
        public PlatformWebView(Func<Window> windowFactory, bool shouldCloseWindow = true)
            : base(windowFactory, shouldCloseWindow)
        {
        }

        /// <inheritdoc/>
        public PlatformWebView(string title = "Authenticating...", int width = 1024, int height = 768)
            : base(title, width, height)
        {
        }
    }
}