using IdentityModel.OidcClient.Browser;
using System.Net.Http;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Specifies options that can be passed to <see cref="Auth0ClientBase"/> implementations.
    /// </summary>
    public class Auth0ClientOptions
    {
        /// <summary>
        /// The <see cref="IBrowser"/> implementation responsible for displaying the Auth0 Login screen. Leave this
        /// unassigned to accept the recommended implementation for platform.
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
        /// Indicates whether basic telemetry information should be included with requests to Auth0.
        /// </summary>
        /// <remarks>
        /// The telemetry information is like a browser user agent and includes operating system
        /// details only to let Auth0 guide engineering resources based on platform popularity.
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
        /// Allow overriding the RetryMessageHandler.
        /// </summary>
        /// <example>
        /// var handler = new HttpClientHandler();
        /// var options = new Auth0ClientOptions
        /// {
        ///    RefreshTokenMessageHandler = handler
        /// };
        /// </example>
        public HttpMessageHandler RefreshTokenMessageHandler { get; set; }

        /// <summary>
        /// Allow overriding the BackchannelHandler.
        /// </summary>
        /// <example>
        /// <code>
        /// var handler = new HttpClientHandler();
        /// var options = new Auth0ClientOptions
        /// {
        ///    BackchannelHandler = handler
        /// };
        /// </code>
        /// </example>
        public HttpMessageHandler BackchannelHandler { get; set; }

        /// <summary>
        /// Override the Redirect URI used to return from logout.
        /// </summary>
        /// <remarks>
        /// Defaults to a platform-specific value you can observe in the debug console window when performing a logout.
        /// On iOS this is made from the app bundle ID and on Android from a lower-cased version of the package name.
        /// Whether you use the default or manually set this value it must be added to the 
        /// Allowed Logout URLs for this application/client to allow the logout process to complete.
        /// </remarks>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Override the the Redirect URI used to return from login.
        /// </summary>
        /// <remarks>
        /// Defaults to a platform-specific value you can observe in the debug console window when performing a login.
        /// On iOS this is made from the app bundle ID and on Android from a lower-cased version of the package name.
        /// Whether you use the default or manually set this value it must be added to the 
        /// Allowed Callback URLs for this application/client to allow the login process to complete.
        /// </remarks>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Create a new instance of the <see cref="Auth0ClientOptions"/> class used to configure options for
        /// <see cref="Auth0ClientBase"/> implementations by way of their constructors.
        /// </summary>
        public Auth0ClientOptions()
        {
            EnableTelemetry = true;
            LoadProfile = true;
            Scope = "openid profile";
        }
    }
}