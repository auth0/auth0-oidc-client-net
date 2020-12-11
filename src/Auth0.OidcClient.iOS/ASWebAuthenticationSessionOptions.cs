namespace Auth0.OidcClient
{
    /// <summary>
    /// Specifies options that can be passed to <see cref="ASWebAuthenticationSessionBrowser"/> implementations.
    /// </summary>
    public class ASWebAuthenticationSessionOptions
    {
        /// <summary>
        /// Specify whether or not EphemeralWebBrowserSessions should be preferred. Defaults to false.
        /// </summary>
        public bool PrefersEphemeralWebBrowserSession { get; set; }
    }
}
