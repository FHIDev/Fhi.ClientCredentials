using System.Text.Json.Serialization;

namespace Fhi.ClientCredentialsKeypairs.Tests.Mocks;

public class OidcMetadata
{
    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = string.Empty;
}