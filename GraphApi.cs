using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string clientId = "your-client-id";
            string clientSecret = "your-client-secret";
            string tenantId = "your-tenant-id";

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .Build();

            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            GraphServiceClient graphClient = new GraphServiceClient(authProvider);
            graphClient.BaseUrl = "https://graph.microsoft.com/beta";

            var sites = await graphClient.Sites.Request().GetAsync();
            foreach (var site in sites)
            {
                Console.WriteLine(site.DisplayName);
            }

            Console.ReadLine();
        }
    }
}
