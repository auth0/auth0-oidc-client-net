using System.Net.Http;
using IdentityModel.OidcClient.Browser;

namespace Auth0.OidcClient
{
    public class Auth0ClientOptions
    {
        /// <summary>
        /// The <see cref="IBrowser"/> implementation which is responsible for displaying the Auth0 Login screen
        /// </summary>
        public IBrowser Browser { get; set; }

        /// <summary>
        /// Your Auth0 Client ID.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Your Auth0 Client Secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Your Auth0 tenant domain.
        /// </summary>
        /// <remarks>
        /// e.g. tenant.auth0.com
        /// </remarks>
        public string Domain { get; set; }

        /// <summary>
        /// Indicates whether telemetry information should be sent to Auth0.
        /// </summary>
        /// <remarks>
        /// Telemetry simply contains information about the version of the Auth0 OIDC Client being used. No information about your
        /// application or users are being sent to Auth0.
        /// </remarks>
        public bool EnableTelemetry { get; set; }

        /// <summary>
        /// Indicates whether the user profile should be loaded from the /userinfo endpoint.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public bool LoadProfile { get; set; }

        /// <summary>
        /// The scopes you want to request.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Allow overriding the RetryMessageHandler
        /// <example>
        /// var handler = new HttpClientHandler();
        /// var options = new Auth0ClientOptions
        /// {
        ///    RefreshTokenMessageHandler = handler
        /// };
        /// </example>
        /// </summary>
        public HttpMessageHandler RefreshTokenMessageHandler { get; set; }

        /// <summary>
        /// Allow overriding the BackchannelHandler
        /// <example>
        /// var handler = new HttpClientHandler();
        /// var options = new Auth0ClientOptions
        /// {
        ///    BackchannelHandler = handler
        /// };
        /// </example>
        /// </summary>
        public HttpMessageHandler BackchannelHandler { get; set; }

        /// <summary>
        /// Allow overriding of the Post Logout Redirect URI
        /// </summary>
        /// <remarks>
        /// This should only be done in exceptional circumstances
        /// </remarks>
        public string PostLogoutRedirectUri { get; set; }

		/// <summary>
		/// Allow overriding of the Redirect URI
		/// </summary>
		/// <remarks>
		/// This should only be done in exceptional circumstances
		/// </remarks>
		public string RedirectUri { get; set; }

        public Auth0ClientOptions()
        {
            EnableTelemetry = true;
            LoadProfile = true;
            Scope = "openid profile";
        }
    }
}