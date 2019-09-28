using System;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Provided for backward compatibility with earlier versions of Auth0.OidcClient.UWP by implementing
    /// <see cref="WebAuthenticationBrokerBrowser"/>.
    /// </summary>
    [Obsolete("It is recommended you leave Browser unassigned to accept the library default or assign an instance of WebAuthenticationBrokerBrowser if you need to enable Windows authentication.")]
    public class PlatformWebView : WebAuthenticationBrokerBrowser
    {
        /// <inheritdoc />
        public PlatformWebView(bool enableWindowsAuthentication = false)
            : base(enableWindowsAuthentication)
        {
        }
    }
}
