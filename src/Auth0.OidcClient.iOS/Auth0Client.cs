﻿using static Foundation.NSBundle;

namespace Auth0.OidcClient
{
    /// <summary>
    /// Primary class for performing authentication and authorization operations with Auth0 using the
    /// underlying <see cref="Duende.IdentityModel.OidcClient.OidcClient"/>.
    /// </summary>
    public class Auth0Client : Auth0ClientBase
    {
        /// <summary>
        /// Creates a new instance of the Auth0 OIDC Client.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration for the Auth0 OIDC Client.</param>
        public Auth0Client(Auth0ClientOptions options)
            : base(options, "xamarin-ios")
        {
            options.Browser = options.Browser ?? new AutoSelectBrowser();
            var callbackUrl = $"{MainBundle.BundleIdentifier}://{options.Domain}/ios/{MainBundle.BundleIdentifier}/callback";
            options.RedirectUri = options.RedirectUri ?? callbackUrl;
            options.PostLogoutRedirectUri = options.PostLogoutRedirectUri ?? callbackUrl;
        }
    }
}
