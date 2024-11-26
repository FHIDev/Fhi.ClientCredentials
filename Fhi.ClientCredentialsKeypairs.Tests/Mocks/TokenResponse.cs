using System.Text.Json.Serialization;

namespace Fhi.ClientCredentialsKeypairs.Tests.Mocks;

public class TokenResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}