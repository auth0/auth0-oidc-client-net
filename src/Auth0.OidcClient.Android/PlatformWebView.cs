using System;

namespace Auth0.OidcClient
{
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default of ChromeCustomTabsBrowser.")]
    public class PlatformWebView : SystemBrowser
    {
    }
}