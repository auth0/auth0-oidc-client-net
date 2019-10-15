using System;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provided for backward compatibility with earlier versions of Auth0.OidcClient.Android by implementing
    /// <see cref="SystemBrowser"/>.
    /// </summary>
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default of ChromeCustomTabsBrowser.")]
    public class PlatformWebView : SystemBrowser
    {
    }
}