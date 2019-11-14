using System.IdentityModel.Tokens.Jwt;

namespace Auth0.OidcClient.Tokens
{
    internal interface ISignatureVerifier
    {
        JwtSecurityToken VerifySignature(string token);
    }
}
