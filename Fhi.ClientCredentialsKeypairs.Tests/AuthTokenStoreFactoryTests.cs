using Fhi.ClientCredentialsKeypairs;
using NSubstitute;

public class AuthTokenStoreFactoryTests
{
    [Test]
    public void Create_ReturnsAuthenticationStore()
    {
        // Arrange
        var config = Substitute.For<ClientCredentialsConfiguration>();
        var api = new Api();
        var factory = new AuthTokenStoreFactory(config);

        // Act
        var result = factory.Create(api);

        // Assert
        Assert.That(result, Is.InstanceOf<AuthenticationStore>());
    }
}