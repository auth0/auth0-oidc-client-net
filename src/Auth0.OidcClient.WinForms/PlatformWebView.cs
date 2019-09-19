using System;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebBrowserBrowser if you need custom form handling.")]
    public class PlatformWebView : WebBrowserBrowser
    {
        public PlatformWebView(Func<Form> formFactory)
            : base(formFactory)
        {
        }

        public PlatformWebView(string title = "Authenticating...", int width = 1024, int height = 768)
            : base(title, width, height)
        {
        }
    }
}