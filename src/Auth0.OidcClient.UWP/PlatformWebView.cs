using System;

namespace Auth0.OidcClient
{
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebAuthenticationBrokerBrowser if you need to enable Windows authentication.")]
    public class PlatformWebView : WebAuthenticationBrokerBrowser
    {
        public PlatformWebView(bool enableWindowsAuthentication = false)
            : base(enableWindowsAuthentication)
        {
        }
    }
}
