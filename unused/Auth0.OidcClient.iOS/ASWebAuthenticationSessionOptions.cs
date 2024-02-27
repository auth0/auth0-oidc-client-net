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
        /// <remarks>
        /// Setting <see cref="PrefersEphemeralWebBrowserSession"/> to true will disable <see href="https://auth0.com/docs/sso">Single Sign On (SSO)</see> on iOS 13+.
        /// As a consequence of that, it will also prevent from showing the popup that's being used to ask consent for using Auth0 to sign in.
        /// </remarks>
        public bool PrefersEphemeralWebBrowserSession { get; set; }
    }
}
