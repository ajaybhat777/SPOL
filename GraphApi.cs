using System;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Http;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Graph;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string clientId = "your-client-id";
            string clientSecret = "your-client-secret";
            string tenantId = "your-tenant-id";

            IAuthenticationProvider authProvider = new ClientCredentialProvider(clientId, clientSecret, tenantId);

            var serializer = new JsonSerializationWriter();

            var httpCore = new HttpCore(new Uri("https://graph.microsoft.com/beta"), new RetryHandler(new Uri("https://graph.microsoft.com/beta")));
            var httpCoreFactory = new HttpCoreFactory(new[] { httpCore });
            var requestAdapter = new RequestAdapter(httpCoreFactory, serializer);

            var graphClient = new GraphServiceClient(requestAdapter);
            graphClient.AuthenticationProvider = authProvider;

            var sites = await graphClient.Sites.Request().GetAsync();
            foreach (var site in sites)
            {
                Console.WriteLine(site.DisplayName);
            }

            Console.ReadLine();
        }
    }
}
