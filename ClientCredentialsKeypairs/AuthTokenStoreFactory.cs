using Microsoft.Extensions.Options;

namespace Fhi.ClientCredentialsKeypairs;

public interface IAuthTokenStoreFactory
{
    IAuthTokenStore Create(Api api);
}

public class AuthTokenStoreFactory(IOptions<ClientCredentialsConfiguration> configuration) : IAuthTokenStoreFactory
{
    public IAuthTokenStore Create(Api api)
    {
        var authService = new AuthenticationService(configuration.Value, api);
        var newStore = new AuthenticationStore(authService, configuration);
        return newStore;
    }
}
