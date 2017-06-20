using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;

namespace Auth0.OidcClient
{
    public class Auth0Client
    {
        private readonly IdentityModel.OidcClient.OidcClient _oidcClient;

        /// <summary>
        /// Creates a new instance of the Auth0 OIDC Client.
        /// </summary>
        /// <param name="options">The <see cref="Auth0ClientOptions"/> specifying the configuration for the Auth0 OIDC Client.</param>
        public Auth0Client(Auth0ClientOptions options)
        {
            var authority = $"https://{options.Domain}";
#if __ANDROID__
            string packageName = options.Activity.Application.ApplicationInfo.PackageName;
#endif
            var oidcClientOptions = new OidcClientOptions
            {
                Authority = authority,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Scope = options.Scope,
                LoadProfile = options.LoadProfile,
#if __IOS__
				RedirectUri = $"{Foundation.NSBundle.MainBundle.BundleIdentifier}://{options.Domain}/ios/{Foundation.NSBundle.MainBundle.BundleIdentifier}/callback",
				Browser = new PlatformWebView(options.Controller),
#elif __ANDROID__
                RedirectUri = $"{packageName}://{options.Domain}/android/{packageName}/callback".ToLower(),
                Browser = new PlatformWebView(options.Activity),
#elif WINDOWS_UWP
                RedirectUri = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri,
                Browser = options.Browser ?? new PlatformWebView(),
#else
                RedirectUri = options.RedirectUri ?? $"https://{options.Domain}/mobile",
                Browser = options.Browser ?? new PlatformWebView(),
#endif
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
#if WINDOWS_UWP
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
#else
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
#endif
                Policy =
                {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };
            _oidcClient = new IdentityModel.OidcClient.OidcClient(oidcClientOptions);
        }

        /// <summary>
        /// Launches a browser to log the user in.
        /// </summary>
        /// <param name="extraParameters">Any extra parameters that need to be passed to the authorization endpoint.</param>
        /// <returns></returns>
        public Task<LoginResult> LoginAsync(object extraParameters = null)
        {
            return _oidcClient.LoginAsync(extraParameters: extraParameters);
        }

        /// <summary>
        /// Generates an <see cref="AuthorizeState"/> containing the URL, state, nonce and code challenge which can
        /// be used to redirect the user to the authorization URL, and subsequently process any response by calling
        /// the <see cref="ProcessResponseAsync"/> method.
        /// </summary>
        /// <param name="extraParameters"></param>
        /// <returns></returns>
        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null)
        {
            return _oidcClient.PrepareLoginAsync(extraParameters);
        }

        /// <summary>
        /// Process the response from the Auth0 redirect URI
        /// </summary>
        /// <param name="data">The data containing the full redirect URI.</param>
        /// <param name="state">The <see cref="AuthorizeState"/> which was generated when the <see cref="PrepareLoginAsync"/>
        /// method was called.</param>
        /// <returns></returns>
        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state)
        {
            return _oidcClient.ProcessResponseAsync(data, state);
        }

        /// <summary>
        /// Generates a new set of tokens based on a refresh token. 
        /// </summary>
        /// <param name="refreshToken">The refresh token which was issued during the authorization flow, or subsequent
        /// calls to <see cref="RefreshTokenAsync"/>.</param>
        /// <returns></returns>
        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            return _oidcClient.RefreshTokenAsync(refreshToken);
        }
    }
}