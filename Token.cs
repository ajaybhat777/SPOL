using System;
using System.Net.Http;
using System.Threading.Tasks;

public async Task<string> GetAccessToken(string clientId, string clientSecret, string tenantId)
{
    string tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
    string[] scopes = { "https://graph.microsoft.com/.default" };

    using (HttpClient client = new HttpClient())
    {
        // Set the client ID and secret for authentication
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

        // Set the form data for the token request
        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", string.Join(" ", scopes))
        });

        // Send the token request and parse the response
        HttpResponseMessage response = await client.PostAsync(tokenEndpoint, formData);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to obtain access token: {responseContent}");
        }

        dynamic tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
        string accessToken = tokenResponse.access_token;

        return accessToken;
    }
}
