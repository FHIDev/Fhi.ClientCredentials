namespace Fhi.ClientCredentialsKeypairs;

public interface ITokenStoreResolver
{
    IAuthTokenStore GetStoreForApi(Api api);
}

public class TokenStoreResolver(IAuthTokenStoreFactory authTokenStoreFactory) : ITokenStoreResolver
{
    private readonly Dictionary<string, IAuthTokenStore> _stores = [];

    public IAuthTokenStore GetStoreForApi(Api api)
    {
        lock (_stores)
        {
            if (_stores.TryGetValue(api.Name, out var store))
            {
                return store;
            }

            var newStore = authTokenStoreFactory.Create(api);

            _stores.Add(api.Name, newStore);
            return newStore;
        }
    }
}
