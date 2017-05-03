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
                RedirectUri = $"https://{options.Domain}/mobile",
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

        public Task<LoginResult> LoginAsync(object extraParameters = null)
        {
            return _oidcClient.LoginAsync(extraParameters: extraParameters);
        }

        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            return _oidcClient.RefreshTokenAsync(refreshToken);
        }
    }
}