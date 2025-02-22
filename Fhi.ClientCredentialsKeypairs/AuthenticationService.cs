using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Advanced;
using Microsoft.Identity.Client.Extensibility;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
    private readonly IClientAssertionService _clientAssertionService;

    public AuthenticationService(ClientCredentialsConfiguration config, Api api, IClientAssertionService clientAssertionService)
    {
        Config = config;
        Client = new HttpClient();
        Api = api;
        _clientAssertionService = clientAssertionService;
    }

    public AuthenticationService(HttpClient client, ClientCredentialsConfiguration config, Api api, IClientAssertionService clientAssertionService)
    {
        Config = config;
        Client = client;
        Api = api;
        _clientAssertionService = clientAssertionService;
    }

    [Obsolete("Use GetAccessToken(HttpMethod method, string url)")]
    public string AccessToken => _accessToken;

    private string _accessToken { get; set; } = "";

    public async Task SetupToken()
    {
        var scopeString = string.IsNullOrEmpty(Api.Scope) ? Config.Scopes : Api.Scope;
        var scopes = scopeString.Split(' ');

        // Remove /connect/token in case the authority config has this appended
        var authorityUri = Config.Authority.Replace("/connect/token", "");

        var confidentialClientApp = ConfidentialClientApplicationBuilder.Create(Config.ClientId)
            .WithOidcAuthority(authorityUri)
            .WithClientAssertion((AssertionRequestOptions _) => _clientAssertionService.CreateClientAssertionJwtAsync(authorityUri, Config.ClientId, Config.PrivateKey))
            .WithExperimentalFeatures()
            .WithHttpClientFactory(new HttpClientContainer(Client))
            .Build();

        var request = confidentialClientApp.AcquireTokenForClient(scopes);

        if (Api.UseDpop)
        {
            request.WithExtraHttpHeaders(new Dictionary<string, string>
            {
                { DPoPHeaderNames.DPoP, BuildDpopAssertion(HttpMethod.Post, Config.Authority) }
            }).WithProofOfPosessionKeyId(Guid.NewGuid().ToString(), "DPoP");
        }

        try
        {
            var result = await request
                .ExecuteAsync();

            _accessToken = result.AccessToken;
        }
        catch (MsalServiceException e) when (e.ErrorCode == DPoPErrorCode.UseDPoPNonce)
        {
            var nonce = e.Headers.GetValues(DPoPHeaderNames.Nonce).FirstOrDefault() ??
                        throw new DPoPException("There must be exactly one value for the DPoP-Nonce header");

            request.WithExtraHttpHeaders(new Dictionary<string, string>
            {
                { DPoPHeaderNames.DPoP, BuildDpopAssertion(HttpMethod.Post, Config.Authority, nonce) }
            });

            var result = await request
                .ExecuteAsync();

            _accessToken = result.AccessToken;
        }
    }

    public JwtAccessToken GetAccessToken(HttpMethod method, string url)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            throw new DPoPException("No access token is set. Unable to create Dpop Proof.");
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

        var securityKey = new JsonWebKey(Config.PrivateKey);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

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

    private class HttpClientContainer(HttpClient client) : IMsalHttpClientFactory
    {
        public HttpClient GetHttpClient() => client;
    }
}

public class JwtAccessToken()
{
    public string AccessToken { get; set; } = "";

    public string TokenType { get; set; } = "";

    public string? DpopProof { get; set; }
    public bool CanFallbackToBearerToken { get; set; }
}
