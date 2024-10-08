﻿using System.Net.Http.Headers;

namespace Fhi.ClientCredentialsKeypairs
{
    public class HttpAuthHandler : DelegatingHandler
    {
        private const string AnonymousOptionKey = "Anonymous";

        private readonly IAuthTokenStore _authTokenStore;

        public HttpAuthHandler(IAuthTokenStore authTokenStore)
        {
            _authTokenStore = authTokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.Options.All(x => x.Key != AnonymousOptionKey))
            {
                var requestUrl = request.RequestUri!.Scheme + "://" + request.RequestUri!.Authority +
                                 request.RequestUri!.LocalPath;
                var token = await _authTokenStore.GetToken(request.Method, requestUrl);
                if (token != null)
                {
                    if (token.TokenType.ToUpper() == AuthenticationScheme.Dpop.ToUpper())
                    {
                        return await SendWithDpopAsync(request, cancellationToken, token);
                    }
                    else 
                    {
                        request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
                    }
                }
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
                    request.Headers.Remove(AuthenticationScheme.Dpop);

                    return await base.SendAsync(request, cancellationToken);
                }
            }

            return dpopResponse;
        }
    }
}
