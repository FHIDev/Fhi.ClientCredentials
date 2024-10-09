using System.Net;
using System.Net.Http.Json;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fhi.ClientCredentialsKeypairs.Tests.Mocks;

public class OauthTestServerHandler : HttpMessageHandler
{
    private HashSet<string> _usedJtis = new();

    public bool EnableDpop { get; set; } = false;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri!.AbsolutePath.EndsWith(".well-known/openid-configuration"))
        {
            return CreateResult(new OidcMetadata { TokenEndpoint = "https://test/oauth/token" });
        }
        
        var headers = request.Headers
            .SelectMany(x => x.Value.Select(y => new { x.Key, Value = y }))
            .ToDictionary(x => x.Key, x => x.Value);
        headers.TryGetValue("DPoP", out var proof);

        var content = await request.Content!.ReadAsStringAsync();
        var parts = content
            .Split('&')
            .Select(x => new { Key = x.Split('=')[0], Value = x.Split('=')[1] })
            .ToDictionary(x => x.Key, x => x.Value);

        if (!EnableDpop)
        {
            return CreateResult(new TokenResponse { AccessToken = "BearerToken", TokenType = "Bearer"});
        }

        if (proof == null)
        {
            return CreateResult("error", "no dpop proof given", status: HttpStatusCode.BadRequest);
        }

        var jti = GetFromJwt(proof, "jti");
        if (string.IsNullOrWhiteSpace(jti))
        {
            return CreateResult("error", "missig unique jti", status: HttpStatusCode.BadRequest);
        }

        var givenNonce = GetFromJwt(proof, "nonce");

        if (string.IsNullOrWhiteSpace(givenNonce))
        {
            var val = Guid.NewGuid().ToString();
            _usedJtis.Add(jti);
            return CreateResult("error", "use_dpop_nonce", val, HttpStatusCode.BadRequest);
        }

        // jtis must be unique per call to prevent dpop replay attacks
        if (_usedJtis.Contains(jti))
        {
            return CreateResult("error", "invalid_nonce", status: HttpStatusCode.BadRequest);
        }

        _usedJtis.Add(jti);

        return CreateResult(new TokenResponse { AccessToken = "DpopToken", TokenType = "DPoP"});
    }

    private string? GetFromJwt(string proof, string type)
    {
        var handler = new JsonWebTokenHandler();
        var jwt = handler.ReadJsonWebToken(proof);
        return jwt.Claims.FirstOrDefault(x => x.Type == type)?.Value;
    }

    private HttpResponseMessage CreateResult(string key, string value, string? nonce = null, HttpStatusCode status = HttpStatusCode.OK)
    {
        var response = new HttpResponseMessage(status);

        response.Content = JsonContent.Create(new Dictionary<string, string>()
        {
            { key, value }
        });

        if (nonce != null)
        {
            response.Headers.Add(DPoPHeaderNames.Nonce, nonce);
        }

        return response;
    }
    
    private HttpResponseMessage CreateResult<T>(T obj)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = JsonContent.Create(obj);
        return response;
    }
}