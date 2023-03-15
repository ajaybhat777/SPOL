using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kiota.Abstractions.Serialization;
using Kiota.Abstractions.Serialization.Converters.Json;
using Kiota.Builder;
using Kiota.Builder.Extensions;
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

            var serializer = new JsonSerializationWriterFactory();
            var deserializer = new JsonSerializationReaderFactory();
            var requestAdapter = new HttpCore.RequestAdapter(httpMessageHandler, serializer, deserializer);

            var client = new GraphServiceClient(requestAdapter);

            var users = await client.Users.Request().GetAsync();

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
            var requestInformation = new RequestInformation
            {
                HttpMethod = request.Method.ToString(),
                URI = request.RequestUri.ToString(),
            };

            await _authenticationProvider.AuthenticateRequestAsync(requestInformation);
            request.RequestUri = new Uri(requestInformation.URI);
            request.Method = new HttpMethod(requestInformation.HttpMethod);

            return await _httpClient.SendAsync(request, cancellationToken);
        }
    }
}
