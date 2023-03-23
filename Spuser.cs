using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

public async Task<JArray> GetListItems(string listName, string username, string password, string siteUrl)
{
    // Construct the endpoint URL for the SharePoint list
    string endpointUrl = $"{siteUrl}/_api/web/lists/getByTitle('{listName}')/items";

    // Create an HTTP client and set the authorization header with the basic authentication credentials
    HttpClient httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

    // Send the request to the SharePoint list using the HTTP client
    HttpResponseMessage response = await httpClient.GetAsync(endpointUrl);

    // Check the response status code to ensure the items were retrieved successfully
    if (response.IsSuccessStatusCode)
    {
        // Read the response content as a string
        string responseContent = await response.Content.ReadAsStringAsync();

        // Parse the response content as JSON and return the items array
        JObject responseJson = JObject.Parse(responseContent);
        JArray items = (JArray)responseJson["value"];
        return items;
    }
    else
    {
        // Items failed to be retrieved
        return null;
    }
}
