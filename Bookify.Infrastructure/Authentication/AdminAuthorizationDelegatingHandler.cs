using Bookify.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication
{
    public sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly KeycloakOptions _keyCloakOptions;

        public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keyCloakOptions)
        {
            _keyCloakOptions = keyCloakOptions.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationToken = await GetAuthorizationToken(cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme,
                authorizationToken.AccessToken);

            var httpResponseMessage = await base.SendAsync(request, cancellationToken);

            httpResponseMessage.EnsureSuccessStatusCode();

            return httpResponseMessage;
        }

        private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
        {
            var authorizationRequestParameter = new KeyValuePair<string, string>[]
            {
                new("client_id", _keyCloakOptions.AdminClientId),
                new("client_secret", _keyCloakOptions.AdminClientSecret),
                new("scope", "openid email"),
                new("grant_type", "client_credentials")
            };

            var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameter);

            var authorizationRequest = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri(_keyCloakOptions.TokenUrl))
            {
                Content = authorizationRequestContent
            };

            var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

            authorizationResponse.EnsureSuccessStatusCode();

            return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>() ??
                throw new ApplicationException();
        }
    }
}
