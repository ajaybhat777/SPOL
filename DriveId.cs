using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GraphApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace these values with your own
            string siteId = "{site-id}";

            // Create an HTTP client
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "{access-token}");

            // Construct the URL to get the drives for the site
            string url = $"https://graph.microsoft.com/v1.0/sites/{siteId}/drives";

            // Send the HTTP request and get the response
            HttpResponseMessage response = await client.GetAsync(url);

            // Read the response content as a JSON object
            string responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);

            // Loop through the drives and find the one for the Images library
            foreach (var drive in jsonResponse.value)
            {
                if (drive.name == "Images")
                {
                    string driveId = drive.id;
                    Console.WriteLine($"Drive ID for Images library: {driveId}");
                    break;
                }
            }
        }
    }
}
