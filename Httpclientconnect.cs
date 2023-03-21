using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

public async void AddListItem(string listName, string bearerToken, string siteUrl, string title, string description)
{
    // Construct the endpoint URL for the SharePoint list
    string endpointUrl = $"{siteUrl}/_api/web/lists/getByTitle('{listName}')/items";

    // Create an HTTP client and set the authorization header with the bearer token
    HttpClient httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

    // Create a new list item object with the provided title and description
    var newItem = new { 
        __metadata = new { type = "SP.Data.TestListListItem" },
        Title = title,
        Description = description
    };

    // Serialize the new list item object to JSON
    string jsonPayload = JsonConvert.SerializeObject(newItem);

    // Create a new StringContent object with the JSON payload and content type
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    // Post the new list item to the SharePoint list using the HTTP client
    HttpResponseMessage response = await httpClient.PostAsync(endpointUrl, content);

    // Check the response status code to ensure the item was added successfully
    if (response.IsSuccessStatusCode)
    {
        // Item was added successfully
    }
    else
    {
        // Item failed to be added
    }
}
