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
        /// <param name="domain">Your Auth0 domain.</param>
        /// <param name="clientId">Your Auth0 Client ID.</param>
        /// <param name="clientSecret">Your Auth0 Client Secret.</param>
        /// <param name="scope">The scope you want to request during authorization.</param>
        /// <param name="loadProfile">Indicates whether the user's profile should be loaded from the UserInfo endpoint.</param>
#if __ANDROID__
        public Auth0Client(string domain, string clientId, Android.App.Activity activity, string clientSecret = null, string scope = "openid profile", bool loadProfile = true)
#else
        public Auth0Client(string domain, string clientId, string clientSecret = null, string scope = "openid profile", bool loadProfile = true)
#endif
        {
            var authority = $"https://{domain}";

            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope,
                LoadProfile = loadProfile,
#if __IOS__
				RedirectUri = $"{Foundation.NSBundle.MainBundle.BundleIdentifier}://callback",
#elif __ANDROID__
                RedirectUri = $"https://{domain}/android/XamarinAndroidTestApp.XamarinAndroidTestApp/callback",
                Browser = new PlatformWebView(activity),
#else
                RedirectUri = $"https://{domain}/mobile",
                Browser = new PlatformWebView(),

#endif
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy =
                {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };
            _oidcClient = new IdentityModel.OidcClient.OidcClient(options);
        }

#if __IOS__

        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null)
        {
            return _oidcClient.PrepareLoginAsync();
        }

        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state)
        {
            return _oidcClient.ProcessResponseAsync(data, state);
        }

#else

        public Task<LoginResult> LoginAsync(object extraParameters = null)
        {
            return _oidcClient.LoginAsync(extraParameters: extraParameters);
        }

#endif

        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            return _oidcClient.RefreshTokenAsync(refreshToken);
        }
    }
}