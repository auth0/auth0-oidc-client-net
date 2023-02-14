using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Auth0.OidcClient.Tokens
{
    class JsonWebKeys
    {
        private readonly HttpMessageHandler backchannel;

        public JsonWebKeys(HttpMessageHandler backchannel = null)
        {
            this.backchannel = backchannel;
        }
        
        public async Task<JsonWebKeySet> GetForIssuer(string issuer)
        {
            var metadataAddress = new UriBuilder(issuer) { Path = "/.well-known/openid-configuration" }.Uri.OriginalString;
            var openIdConfiguration = await GetOpenIdConfiguration(metadataAddress);
            return openIdConfiguration.JsonWebKeySet;
        }

        private Task<OpenIdConnectConfiguration> GetOpenIdConfiguration(string metadataAddress)
        {
            try
            {
                var configurationManager = backchannel == null ? new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever()) : new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever(), new System.Net.Http.HttpClient(backchannel));
                return configurationManager.GetConfigurationAsync();
            }
            catch (Exception e)
            {
                throw new IdTokenValidationException($"Unable to retrieve public keys from \"${metadataAddress}\".", e);
            }
        }
    }
}