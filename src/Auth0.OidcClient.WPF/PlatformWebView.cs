using System;
using System.Windows;

namespace Auth0.OidcClient
{
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebBrowserBrowser if you need custom window handling.")]
    public class PlatformWebView : WebBrowserBrowser
    {
        public PlatformWebView(Func<Window> windowFactory, bool shouldCloseWindow = true)
            : base(windowFactory, shouldCloseWindow)
        {
        }

        public PlatformWebView(string title = "Authenticating...", int width = 1024, int height = 768)
            : base(title, width, height)
        {
        }
    }
}