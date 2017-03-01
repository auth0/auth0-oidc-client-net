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
        private readonly string _domain;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public Auth0Client(string domain, string clientId)
            : this(domain, clientId, null)
        {
        }

        public Auth0Client(string domain, string clientId, string clientSecret)
        {
            _domain = domain;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private IdentityModel.OidcClient.OidcClient CreateClient(string scope)
        {
            var authority = $"https://{_domain}";

            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Scope = scope,
                RedirectUri = $"https://{_domain}/mobile",
                Browser = new PlatformWebView(),
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy =
                {
                    RequireAuthorizationCodeHash = false,
                    RequireAccessTokenHash = false
                }
            };

            return new IdentityModel.OidcClient.OidcClient(options);
        }

        public Task<LoginResult> LoginAsync(string scope = "openid profile", object extraParameters = null)
        {
            var oidcClient = CreateClient(scope);

            return oidcClient.LoginAsync(extraParameters: extraParameters);
        }

        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            var oidcClient = CreateClient("openid profile");

            return oidcClient.RefreshTokenAsync(refreshToken);
        }
    }
}