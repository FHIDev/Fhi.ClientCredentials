using System.Net.Http.Headers;

namespace Fhi.ClientCredentialsKeypairs
{
    public class HttpAuthHandler(IAuthTokenStore authTokenStore) : DelegatingHandler
    {
        private const string AnonymousOptionKey = "Anonymous";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Options.All(x => x.Key != AnonymousOptionKey))
            {
                var requestUrl = request.RequestUri!.Scheme + "://" + request.RequestUri!.Authority +
                                 request.RequestUri!.LocalPath;
                var token = await authTokenStore.GetToken(request.Method, requestUrl);
                
                if (token.TokenType.ToUpper() == AuthenticationScheme.Dpop.ToUpper())
                {
                    return await SendWithDpopAsync(request, cancellationToken, token);
                }

                request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private async Task<HttpResponseMessage> SendWithDpopAsync(HttpRequestMessage request,
            CancellationToken cancellationToken, JwtAccessToken token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);

            request.Headers.TryAddWithoutValidation(DPoPHeaderNames.DPoP, token.DpopProof);

            var dpopResponse = await base.SendAsync(request, cancellationToken);

            if (token.CanFallbackToBearerToken && dpopResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var supportedSchemes = dpopResponse.Headers.WwwAuthenticate.Select(x => x.Scheme).ToArray();

                if (!supportedSchemes.Contains(AuthenticationScheme.Dpop, StringComparer.InvariantCultureIgnoreCase))
                {
                    // downgrade request to Dpop if Dpop is not supported
                    request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme.Bearer, token.AccessToken);
                    request.Headers.Remove(DPoPHeaderNames.DPoP);

                    return await base.SendAsync(request, cancellationToken);
                }
            }

            return dpopResponse;
        }
    }
}
