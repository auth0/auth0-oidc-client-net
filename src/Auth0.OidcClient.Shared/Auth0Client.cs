using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient;

namespace Auth0.OidcClient
{
    public class Auth0Client
    {
        private readonly string _domain;
        private readonly string _clientId;

        public Auth0Client(string domain, string clientId)
        {
            _domain = domain;
            _clientId = clientId;
        }

        public async Task LoginAsync()
        {
            var authority = $"https://{_domain}";

            var options = new OidcClientOptions
            {
                Authority = authority,
                ClientId = _clientId,
                Scope = "openid profile",
                RedirectUri = $"https://{_domain}/mobile",
                Browser = new PlatformWebView(),
                Flow = OidcClientOptions.AuthenticationFlow.Hybrid,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };
            options.Policy.RequireAuthorizationCodeHash = false;
            options.Policy.RequireAccessTokenHash = false;

            var oidcClient = new IdentityModel.OidcClient.OidcClient(options);

            var result = await oidcClient.LoginAsync();
        }
    }
}
