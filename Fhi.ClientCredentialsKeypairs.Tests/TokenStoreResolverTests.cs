using NSubstitute;

namespace Fhi.ClientCredentialsKeypairs.Tests;

public class TokenStoreResolverTests
{
    private IAuthTokenStoreFactory _authTokenStoreFactory;
    private TokenStoreResolver _resolver;
    private IAuthTokenStore _authTokenStore;

    [SetUp]
    public void SetUp()
    {
        _authTokenStoreFactory = Substitute.For<IAuthTokenStoreFactory>();
        _authTokenStore = Substitute.For<IAuthTokenStore>();
        _resolver = new TokenStoreResolver(_authTokenStoreFactory);
    }

    [Test]
    public void GetStoreForApi_ReturnsStoreFromFactory_WhenNotCached()
    {
        // Arrange
        var api = new Api { Name = "TestApi" };
        _authTokenStoreFactory.Create(api).Returns(_authTokenStore);

        // Act
        var result = _resolver.GetStoreForApi(api);

        // Assert
        Assert.That(result, Is.EqualTo(_authTokenStore));
    }

    [Test]
    public void GetStoreForApi_ReturnsCachedStore_WhenStoreExists()
    {
        // Arrange
        var api = new Api { Name = "TestApi" };
        _authTokenStoreFactory.Create(api).Returns(_authTokenStore);

        // Act
        var firstCallResult = _resolver.GetStoreForApi(api);
        var secondCallResult = _resolver.GetStoreForApi(api);

        // Assert
        Assert.That(firstCallResult, Is.EqualTo(secondCallResult));
        _authTokenStoreFactory.Received(1).Create(api);
    }
}