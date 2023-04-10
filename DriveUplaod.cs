using (var httpClient = new HttpClient())
{
    var accessToken = "INSERT_ACCESS_TOKEN_HERE";
    var siteName = "INSERT_SITE_NAME_HERE";
    var listName = "INSERT_LIST_NAME_HERE";
    var filename = "INSERT_FILENAME_HERE";

    // Step 1: Get the site ID
    var requestUrl = $"https://graph.microsoft.com/v1.0/sites/{siteName}";
    var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    var response = await httpClient.SendAsync(request, new CancellationToken());
    response.EnsureSuccessStatusCode();
    var json = await response.Content.ReadAsStringAsync();
    var site = JsonConvert.DeserializeObject<Site>(json);

    // Step 2: Get the list ID
    requestUrl = $"https://graph.microsoft.com/v1.0/sites/{site.Id}/lists?$filter=displayName eq '{listName}'";
    request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    response = await httpClient.SendAsync(request, new CancellationToken());
    response.EnsureSuccessStatusCode();
    json = await response.Content.ReadAsStringAsync();
    var lists = JsonConvert.DeserializeObject<ListCollectionResponse>(json);
    var list = lists.Value.FirstOrDefault();

    // Step 3: Get the ID of the folder to which you want to upload the image
    requestUrl = $"https://graph.microsoft.com/beta/sites/{site.Id}/drive/root:/{listName}";
    request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    response = await httpClient.SendAsync(request, new CancellationToken());
    response.EnsureSuccessStatusCode();
    json = await response.Content.ReadAsStringAsync();
    var folder = JsonConvert.DeserializeObject<DriveItem>(json);

    // Step 4: Create a new file in the folder
    requestUrl = $"https://graph.microsoft.com/beta/sites/{site.Id}/drive/items/{folder.Id}:/{filename}:/content";
    var imageBytes = File.ReadAllBytes("INSERT_FILE_PATH_HERE");
    using (var content = new ByteArrayContent(imageBytes))
    {
        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Replace with appropriate image format
        request = new HttpRequestMessage(HttpMethod.Put, requestUrl);
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        response = await httpClient.SendAsync(request, new CancellationToken());
        response.EnsureSuccessStatusCode();
        json = await response.Content.ReadAsStringAsync();
        var uploadedFile = JsonConvert.DeserializeObject<DriveItem>(json);

        // Step 5: Set the metadata for the file
        requestUrl = $"https://graph.microsoft.com/beta/sites/{site.Id}/drive/items/{uploadedFile.Id}/listItem/fields";
        var metadata = new Dictionary<string, object>()
        {
            { "Title", "My Image Title" },
            { "Description", "My Image Description" },
            { "Author", "My Name" },
            { "Keywords", new string[] { "image", "metadata", "graph api" } }
        };
        var metadataJson = JsonConvert.SerializeObject(metadata);
        var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
        request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl);
        request.Content = metadataContent;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        response = await httpClient.SendAsync(request, new CancellationToken());
        response.EnsureSuccessStatusCode();
        json = await response.Content.ReadAsStringAsync();
        var updatedMetadata = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
    }
}
