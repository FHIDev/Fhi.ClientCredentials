namespace Fhi.ClientCredentialsKeypairs
{
    public interface IAuthTokenStore
    {
        [Obsolete("Use GetToken(HttpMethod method, string url)")]
        Task<string> GetToken();

        Task<JwtAccessToken> GetToken(HttpMethod method, string url);
    }

    public class AuthenticationStore(
        IAuthenticationService authenticationService,
        ClientCredentialsConfiguration configuration)
        : IAuthTokenStore
    {
        private DateTime tokenDateTime = DateTime.MinValue;
        private readonly int refreshTokenAfterMinutes = configuration.RefreshTokenAfterMinutes;

        [Obsolete("Use GetToken(HttpMethod method, string url)")]
        public async Task<string> GetToken()
        {
            if ((DateTime.Now - tokenDateTime).TotalMinutes > refreshTokenAfterMinutes)
            {
                await Refresh();
            }

            return authenticationService.AccessToken;
        }

        public async Task<JwtAccessToken> GetToken(HttpMethod method, string url)
        {
            if ((DateTime.Now - tokenDateTime).TotalMinutes > refreshTokenAfterMinutes)
            {
                await Refresh();
            }

            return authenticationService.GetAccessToken(method, url);
        }

        private async Task Refresh()
        {
            await authenticationService.SetupToken();
            tokenDateTime = DateTime.Now;
        }
    }
}
