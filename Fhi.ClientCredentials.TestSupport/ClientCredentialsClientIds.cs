using Fhi.ClientCredentialsKeypairs;
using Microsoft.Extensions.Configuration;

namespace Fhi.ClientCredentials.TestSupport;

public class ClientCredentialsClientIds : SetupBaseConfigTests
{
    public ClientCredentialsConfiguration ClientCredentialsConfiguration { get; set; }
    public ClientCredentialsClientIds(string configFile, AppSettingsUsage useOfAppsettings) : base(configFile, useOfAppsettings)
    {
        ClientCredentialsConfiguration = Config.GetSection(nameof(ClientCredentialsKeypairs.ClientCredentialsConfiguration))
            .Get<ClientCredentialsConfiguration>() ?? throw new Exception($"No section with name {nameof(ClientCredentialsKeypairs.ClientCredentialsConfiguration)} found");
    }

    protected override void Guard()
    {
        // Does nothing here
    }
}