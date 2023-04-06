using Microsoft.Graph;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Program
{
    static async Task Main(string[] args)
    {
        string siteName = "your-site-name";
        string listName = "your-list-name";
        string accessToken = "{access-token}";

        // Retrieve the site
        var siteUrl = $"https://{siteName}.sharepoint.com";
        var siteRequestUrl = $"https://graph.microsoft.com/beta/sites?url={siteUrl}";
        var siteResponse = await SendGraphRequestAsync(siteRequestUrl, accessToken);
        var site = JsonConvert.DeserializeObject<Site>(siteResponse.ToString());

        // Retrieve the list
        var listRequestUrl = $"https://graph.microsoft.com/beta/sites/{site.Id}/lists?$filter=DisplayName eq '{listName}'";
        var listResponse = await SendGraphRequestAsync(listRequestUrl, accessToken);
        var list = JsonConvert.DeserializeObject<List>(listResponse["value"][0].ToString());

        // Construct the request URL
        var itemRequestUrl = $"https://graph.microsoft.com/beta/sites/{site.Id}/lists/{list.Id}/items";

        // Construct the request body
        var item = new
        {
            fields = new
            {
                Title = "New item title",
                Description = "New item description"
            }
        };
        var json = JsonConvert.SerializeObject(item);
        var body = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Add the request headers
        var requestHeaders = new
        {
            Authorization = new AuthenticationHeaderValue("Bearer", accessToken),
            Content_Type = new MediaTypeHeaderValue("application/json")
        };

        // Send the request
        var response = await SendGraphRequestAsync(itemRequestUrl, accessToken, body, HttpMethod.Post, requestHeaders);
        var itemId = response["id"].ToString();

        Console.WriteLine($"New item created with ID {itemId}");
    }

    static async Task<JObject> SendGraphRequestAsync(string url, string accessToken, HttpContent body = null, HttpMethod method = null, object headers = null)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (headers != null)
            {
                var headersDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(headers));
                foreach (var header in headersDict)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            var httpRequestMessage = new HttpRequestMessage(method ?? HttpMethod.Get, url);
            httpRequestMessage.Content = body;

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<JObject>(responseContent);
        }
    }
}
