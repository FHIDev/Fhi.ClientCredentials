﻿using System.Net.Http.Headers;

namespace Fhi.ClientCredentialsKeypairs
{
    public class HttpAuthHandler : DelegatingHandler
    {

        private readonly IAuthTokenStore _authTokenStore;
        public HttpAuthHandler(IAuthTokenStore authTokenStore)
        {
            _authTokenStore = authTokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string token = _authTokenStore.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

    }
}
