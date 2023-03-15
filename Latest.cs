using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;
using System;
using System.Threading.Tasks;

namespace SharePointOnlineGraphApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = "your-client-id";
            var clientSecret = "your-client-secret";
            var tenantId = "your-tenant-id";

            var clientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authenticationResult = await clientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
            var accessToken = authenticationResult.AccessToken;

            var authProvider = new DelegateAuthenticationProvider(async (requestMessage) =>
            {
                requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            });

            var graphClient = new GraphServiceClient(authProvider, new[] { new JsonSerializationWriterFactory(), new JsonSerializationReaderFactory() });

            var listId = "your-list-id";
            var items = await graphClient.Sites["root"].Lists[listId].Items.Request().GetAsync();

            foreach (var item in items)
            {
                Console.WriteLine(item.Id);
            }
        }
    }
}
