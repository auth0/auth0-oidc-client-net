using System;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provided for backward compatibility with earlier versions of Auth0.OidcClient.iOS by implementing
    /// <see cref="AutoSelectBrowser"/>.
    /// </summary>
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default of AutoSelectBrowser.")]
    public class PlatformWebView : AutoSelectBrowser
    {
    }
}
