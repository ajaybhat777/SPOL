using (var httpClient = new HttpClient())
{
    using (var content = new ByteArrayContent(imageBytes))
    {
        var accessToken = "INSERT_ACCESS_TOKEN_HERE";
        var siteId = "INSERT_SITE_ID_HERE";
        var folderPath = "INSERT_FOLDER_PATH_HERE";
        var filename = "INSERT_FILENAME_HERE";

        // Step 1: Get the ID of the folder to which you want to upload the image
        var requestUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drive/root:/{folderPath}";
        var response = await httpClient.GetAsync(requestUrl, new CancellationToken());
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var folder = JsonConvert.DeserializeObject<DriveItem>(json);

        // Step 2: Create a new file in the folder
        requestUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drive/items/{folder.Id}:/{filename}:/content";
        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Replace with appropriate image format
        var uploadResponse = await httpClient.PutAsync(requestUrl, content, new CancellationToken());
        uploadResponse.EnsureSuccessStatusCode();
        var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
        var uploadedFile = JsonConvert.DeserializeObject<DriveItem>(uploadJson);

        // Step 3: Set the metadata for the file
        requestUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drive/items/{uploadedFile.Id}/listItem/fields";
        var metadata = new Dictionary<string, object>()
        {
            { "Title", "My Image Title" },
            { "Description", "My Image Description" },
            { "Author", "My Name" },
            { "Keywords", new string[] { "image", "metadata", "graph api" } }
        };
        var metadataJson = JsonConvert.SerializeObject(metadata);
        var metadataContent = new StringContent(metadataJson, Encoding.UTF8, "application/json");
        var patchRequest = new HttpRequestMessage(new HttpMethod("PATCH"), requestUrl);
        patchRequest.Content = metadataContent;
        patchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var metadataResponse = await httpClient.SendAsync(patchRequest, new CancellationToken());
        metadataResponse.EnsureSuccessStatusCode();
    }
}
