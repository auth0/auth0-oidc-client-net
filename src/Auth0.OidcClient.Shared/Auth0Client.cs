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

        public async Task LoginAsync()
        {
            var authority = $"https://{_domain}";

            var options = new OidcClientOptions(
                authority,
                _clientId,
                _clientSecret,
                "openid profile",
                redirectUri: $"https://{_domain}/mobile",
                webView: new PlatformWebView()
            );
            options.Style = OidcClientOptions.AuthenticationStyle.AuthorizationCode;
            options.UseFormPost = false;
            //options.Policy.RequireAuthorizationCodeHash = false;
            //options.Policy.RequireAccessTokenHash = false;

            var oidcClient = new IdentityModel.OidcClient.OidcClient(options);

            var result = await oidcClient.LoginAsync(extraParameters: new {audience = "https://rs256.test.api"});
        }
    }
}