﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fhi.ClientCredentialsKeypairs
{
    /// <summary>
    /// Genereate client assertion used when getting access token
    /// </summary>
    public interface IClientAssertionService
    {
        /// <summary>
        /// Build jwt client assertion value
        /// </summary>
        /// <param name="issuer">token issuer</param>
        /// <param name="clientId">client id that will be set at jwt issuer</param>
        /// <param name="privateKey">the key used to sign the jwt</param>
        /// <returns>jwt string</returns>
        public Task<string> CreateClientAssertionJwtAsync(string issuer, string clientId, string privateKey);
    }

    public class ClientAssertionService : IClientAssertionService
    {
        public Task<string> CreateClientAssertionJwtAsync(string issuer, string clientId, string privateKey)
        {
            var clientAssertion = BuildClientAssertion(issuer, clientId, privateKey);

            return Task.FromResult(clientAssertion);
        }

        private string BuildClientAssertion(string issuer, string clientId, string privateKey)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, clientId),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };

            var signingCredentials = GetClientAssertionSigningCredentials(privateKey);
            var payload = new JwtPayload(clientId, issuer, claims, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(60));
            var header = new JwtHeader(signingCredentials, null, "client-authentication+jwt");
            var jwtSecurityToken = new JwtSecurityToken(header, payload);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
        private SigningCredentials GetClientAssertionSigningCredentials(string privateKey)
        {
            var securityKey = new JsonWebKey(privateKey);
            if (string.IsNullOrEmpty(securityKey.Alg))
                securityKey.Alg = SecurityAlgorithms.RsaSha256;
            return new SigningCredentials(securityKey, securityKey.Alg);
        }
    }
}