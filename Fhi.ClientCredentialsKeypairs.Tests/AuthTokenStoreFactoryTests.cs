using Fhi.ClientCredentialsKeypairs;
using Microsoft.Extensions.Options;
using NSubstitute;

public class AuthTokenStoreFactoryTests
{
    private IOptions<ClientCredentialsConfiguration> _config;
    private Api _api;
    private AuthTokenStoreFactory _factory;

    [SetUp]
    public void SetUp()
    {
        _config = Substitute.For<IOptions<ClientCredentialsConfiguration>>();
        _api = new Api();
        _factory = new AuthTokenStoreFactory(_config);
    }

    [Test]
    public void Create_ReturnsAuthenticationStore()
    {
        // Arrange
        var configValue = Substitute.For<ClientCredentialsConfiguration>();
        _config.Value.Returns(configValue);

        // Act
        var result = _factory.Create(_api);

        // Assert
        Assert.That(result, Is.InstanceOf<AuthenticationStore>());
    }
}