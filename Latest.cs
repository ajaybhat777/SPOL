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
            var tenantId = "your-tenant-id";

            var clientApplication = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };
            var authenticationResult = await clientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
            var accessToken = authenticationResult.AccessToken;

            var authProvider = new TokenCredentialAuthProvider(accessToken);
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
