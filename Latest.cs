using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Kiota.Abstractions.Serialization;
using Kiota.Builder;
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

            var httpClient = new HttpClient(new AuthHandler(authenticationProvider));
            var serializer = new JsonSerializationWriterFactory();
            var deserializer = new JsonSerializationReaderFactory();
            var requestAdapter = new Kiota.Abstractions.RequestAdapter(httpClient, serializer, deserializer);

            var client = new GraphServiceClient(requestAdapter);

            var users = await client.Users.Request().GetAsync();

            foreach (var user in users)
            {
                Console.WriteLine(user.DisplayName);
            }

            Console.ReadLine();
        }
    }

    public class AuthHandler : DelegatingHandler
    {
        private readonly IAuthenticationProvider _authenticationProvider;

        public AuthHandler(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            await _authenticationProvider.AuthenticateRequestAsync(request);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
