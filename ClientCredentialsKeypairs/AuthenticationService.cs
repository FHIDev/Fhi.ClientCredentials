﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;

namespace Fhi.ClientCredentialsKeypairs;

public interface IAuthenticationService
{
    [Obsolete("Use GetAccessToken(HttpMethod method, string url)")]
    string AccessToken { get; }

    JwtAccessToken GetAccessToken(HttpMethod method, string url);

    Task SetupToken();
}

public class AuthenticationService : IAuthenticationService
{
    public ClientCredentialsConfiguration Config { get; }

    public HttpClient Client { get; }

    public Api Api { get; }

    public AuthenticationService(ClientCredentialsConfiguration config, Api api)
    {
        Config = config;
        Client = new HttpClient();
        Api = api;
    }

    public AuthenticationService(HttpClient client, ClientCredentialsConfiguration config, Api api)
    {
        Config = config;
        Client = client;
        Api = api;
    }

    [Obsolete("Use GetAccessToken(HttpMethod method, string url)")]
    public string AccessToken => _accessToken;

    private string _accessToken { get; set; } = "";

    public async Task SetupToken()
    {
        var scopes = string.IsNullOrEmpty(Api.Scope) ? Config.Scopes : Api.Scope;
        var cctr = new ClientCredentialsTokenRequest
        {
            Address = Config.Authority,
            ClientId = Config.ClientId,
            DPoPProofToken = Api.UseDpop ? BuildDpopAssertion(HttpMethod.Post, Config.Authority) : null,
            GrantType = OidcConstants.GrantTypes.ClientCredentials,
            ClientCredentialStyle = ClientCredentialStyle.PostBody,
            Scope = scopes,
            ClientAssertion = new ClientAssertion
            {
                Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                Value = BuildClientAssertion(Config.Authority, Config.ClientId)
            }
        };

        var response = await Client.RequestClientCredentialsTokenAsync(cctr);
        if (response.IsError)
        {
            if (Api.UseDpop && response.Error == OidcConstants.TokenErrors.UseDPoPNonce)
            {
                cctr.DPoPProofToken = BuildDpopAssertion(HttpMethod.Post, Config.Authority, nonce: response.DPoPNonce ?? Guid.NewGuid().ToString());
                response = await Client.RequestClientCredentialsTokenAsync(cctr);
            }

            if (response.IsError)
            {
                throw new Exception($"Unable to get access token: {response.Error}");
            }
        }

        _accessToken = response!.AccessToken ?? "";
    }
    
    public JwtAccessToken GetAccessToken(HttpMethod method, string url)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            throw new Exception("No access token is set. Unable to create Dpop Proof.");
        }

        if (!Api.UseDpop)
        {
            return new JwtAccessToken()
            {
                AccessToken = _accessToken,
                TokenType = "Bearer"
            };
        }

        var ath = CreateDpopAth(_accessToken);

        return new JwtAccessToken()
        {
            AccessToken = _accessToken,
            TokenType = "DPoP",
            DpopProof = BuildDpopAssertion(method, url, ath: ath),
            CanFallbackToBearerToken = Config.CanFallbackToBearerToken
        };
    }

    /// <summary>
    /// Hash of the access token. The value MUST be the result of a base64url encoding (as defined in Section 2 of [RFC7515]) the SHA-256 [SHS] hash of the ASCII encoding of the associated access token's value.
    /// </summary>
    private static string? CreateDpopAth(string accessToken)
    {
        using var encryptor = SHA256.Create();
        var input = Encoding.ASCII.GetBytes(accessToken);
        var sha256 = encryptor.ComputeHash(input);
        return Base64UrlEncoder.Encode(sha256);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nonce">Unique id provided by HelseId upon request. Only used during request to HelseId</param>
    /// <param name="ath">Hash of the AccessToken. Only used when making request to an API with an AccessToken.</param>
    /// <returns></returns>
    private string BuildDpopAssertion(HttpMethod method, string url, string? nonce = null, string? ath = null)
    {
        var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new("jti", jti),
            new("htm", method.ToString().ToUpperInvariant()),
            new("htu", url),
            new("iat", iat.ToString(), ClaimValueTypes.Integer64),
        };

        if (ath != null)
        {
            claims.Add(new("ath", ath));
        }

        if (nonce != null)
        {
            claims.Add(new("nonce", nonce));
        }

        var signingCredentials = GetClientAssertionSigningCredentials();

        var jwtSecurityToken = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);
        jwtSecurityToken.Header.Remove("typ");
        jwtSecurityToken.Header.Add("typ", "dpop+jwt");
        var jwkValue = GetPublicJwk();
        jwtSecurityToken.Header.Add("jwk", jwkValue);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    private Dictionary<string, string> GetPublicJwk()
    {
        var jwk = new JsonWebKey(Config.PrivateKey);
        return new Dictionary<string, string>
        {
            { JsonWebKeyParameterNames.Alg, jwk.Alg },
            { JsonWebKeyParameterNames.E, jwk.E },
            { JsonWebKeyParameterNames.Kty, jwk.Kty },
            { JsonWebKeyParameterNames.N, jwk.N }
        }
        .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    private string BuildClientAssertion(string audience, string clientId)
    {
        var claims = new List<Claim>
        {
            new(JwtClaimTypes.Subject, clientId),
            new(JwtClaimTypes.IssuedAt, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
            new(JwtClaimTypes.JwtId, Guid.NewGuid().ToString("N")),
        };

        var signingCredentials = GetClientAssertionSigningCredentials();
        var jwtSecurityToken = new JwtSecurityToken(clientId, audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(60), signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return token;
    }

    private SigningCredentials GetClientAssertionSigningCredentials()
    {
        var securityKey = new JsonWebKey(Config.PrivateKey);
        return new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
    }
}

public class JwtAccessToken()
{
    public string AccessToken { get; set; } = "";

    public string TokenType { get; set; } = "";

    public string? DpopProof { get; set; }
    public bool CanFallbackToBearerToken { get; set; }
}
