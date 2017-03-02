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

        public Auth0Client(string domain, string clientId, string clientSecret = null, string scope = "openid profile")
        {
            var authority = $"https://{domain}";

            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope,
                RedirectUri = $"https://{domain}/mobile",
                Browser = new PlatformWebView(),
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