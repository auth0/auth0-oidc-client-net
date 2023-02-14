﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;

namespace Auth0.OidcClient.Tokens
{
    internal class AsymmetricSignatureVerifier : ISignatureVerifier
    {
        private readonly IList<JsonWebKey> keys;
        private readonly JsonWebKeys jsonWebKeys;

        public AsymmetricSignatureVerifier(HttpMessageHandler backchannel = null)
        {
            jsonWebKeys = new JsonWebKeys(backchannel);
        }

        public async Task<AsymmetricSignatureVerifier> ForJwks(string issuer)
        {
            var jsonWebKeysSet = await jsonWebKeys.GetForIssuer(issuer);
            return new AsymmetricSignatureVerifier(jsonWebKeysSet.Keys);
        }

        public AsymmetricSignatureVerifier(IList<JsonWebKey> keys)
        {
            this.keys = keys;
        }

        public JwtSecurityToken VerifySignature(string token)
        {
            var securityTokenHandler = new JwtSecurityTokenHandler();

            JwtSecurityToken decoded;
            try
            {
                decoded = securityTokenHandler.ReadJwtToken(token);
            }
            catch (ArgumentException e)
            {
                throw new IdTokenValidationException("ID token could not be decoded.", e);
            }

            if (decoded.SignatureAlgorithm != "RS256")
                throw new IdTokenValidationException($"Signature algorithm of \"{decoded.Header.Alg }\" is not supported. Expected the ID token to be signed with \"RS256\".");

            return (JwtSecurityToken)ValidateTokenSignature(token, securityTokenHandler, decoded.Header.Kid);
        }

        internal SecurityToken ValidateTokenSignature(string token, JwtSecurityTokenHandler securityTokenHandler, string kid)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = false,
                    ValidateTokenReplay = false,

                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = keys,
                };

                securityTokenHandler.ValidateToken(token, validationParameters, out var verifiedToken);
                return verifiedToken;
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                throw new IdTokenValidationException($"Could not find a public key for Key ID (kid) \"{kid}\".");
            }
            catch (SecurityTokenException e)
            {
                throw new IdTokenValidationException("Invalid token signature.", e);
            }
        }
    }
}
