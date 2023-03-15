using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace GraphApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "YOUR_CLIENT_ID";
            var clientSecret = "YOUR_CLIENT_SECRET";
            var tenantId = "YOUR_TENANT_ID";

            var confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            var authenticationProvider = new ClientCredentialProvider(confidentialClientApplication, scopes);

            var httpClient = new HttpClient();
            var httpMessageHandler = new AuthHandler(authenticationProvider, httpClient);
            var graphClient = new GraphServiceClient(httpMessageHandler);

            var users = await graphClient.Users.Request().GetAsync();

            foreach (var user in users)
            {
                Console.WriteLine(user.DisplayName);
            }

            Console.ReadLine();
        }
    }

    public class AuthHandler : HttpMessageHandler
    {
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly HttpClient _httpClient;

        public AuthHandler(IAuthenticationProvider authenticationProvider, HttpClient httpClient)
        {
            _authenticationProvider = authenticationProvider;
            _httpClient = httpClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            await _authenticationProvider.AuthenticateRequestAsync(request);
            return await _httpClient.SendAsync(request, cancellationToken);
        }
    }
}
