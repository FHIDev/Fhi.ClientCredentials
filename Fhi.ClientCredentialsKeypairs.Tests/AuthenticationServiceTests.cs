using Fhi.ClientCredentialsKeypairs.Tests.Mocks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fhi.ClientCredentialsKeypairs.Tests;

public class AuthenticationServiceTests
{
    [Test]
    public async Task CanGetBearerToken()
    {
        var service = GetAuthenticationService(clientUseDpop: false, serverUseDpop: false);
        await service.SetupToken();

        var token = service.GetAccessToken(HttpMethod.Get, "https://test/help");
        Assert.That(token.AccessToken, Is.EqualTo("BearerToken"));
    }

    [Test]
    public async Task CanGetDpopToken()
    {
        var service = GetAuthenticationService(clientUseDpop: true, serverUseDpop: true);
        await service.SetupToken();

        var token = service.GetAccessToken(HttpMethod.Get, "https://test/help");
        Assert.That(token.AccessToken, Is.EqualTo("DpopToken"));
    }

    [Test]
    public async Task DpopTokenIsSha256Base64UrlEncoded()
    {
        var service = GetAuthenticationService(clientUseDpop: true, serverUseDpop: true);
        await service.SetupToken();

        var token = service.GetAccessToken(HttpMethod.Get, "https://test/help");
        
        var handler = new JsonWebTokenHandler();
        var parsedToken  = handler.ReadJsonWebToken(token.DpopProof);
        
        var dpopAthClaim = parsedToken.Claims.Single(t => t.Type == "ath");

        // Assert that the calculated ath claim value is a base64 url encoded value of the sha256 of the access token
        Assert.That(dpopAthClaim.Value, Is.EqualTo("YdX4GGm956hxYFopv8SFiwpL9d0bg4-qhVAWT0s5PBs"));
    }

    private AuthenticationService GetAuthenticationService(bool clientUseDpop, bool serverUseDpop)
    {
        var client = new HttpClient(new OauthTestServerHandler() { EnableDpop = serverUseDpop });

        var api = new Api
        {
            UseDpop = clientUseDpop,
        };

        var service = new AuthenticationService(client, new ClientCredentialsConfiguration()
        {
            authority = "https://test/oauth",
            clientId = "TEST",
            privateJwk = CreateJsonPrivateJwk()
        }, api);

        return service;
    }

    private string CreateJsonPrivateJwk()
    {
        var rsa = RSA.Create(2048);
        var privateKey = new RsaSecurityKey(rsa.ExportParameters(true)) { KeyId = "TestKeyId" };
        var jwk1 = JsonWebKeyConverter.ConvertFromRSASecurityKey(privateKey);
        return JsonSerializer.Serialize(jwk1);
    }
}
