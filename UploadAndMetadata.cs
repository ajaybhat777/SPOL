using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GraphApiExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace these values with your own
            string siteId = "{site-id}";
            string driveId = "{drive-id}";
            string filename = "image.jpg";
            string filePath = @"C:\path\to\image.jpg";
            string description = "This is an image";

            // Create an HTTP client
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "{access-token}");

            // Construct the URL to upload the file and set the metadata
            string uploadUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drives/{driveId}/root:/Images/{filename}:/content";
            string metadataUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drives/{driveId}/root:/Images/{filename}:/listItem/fields";

            // Read the file into a byte array
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Define the metadata properties to set
            var metadata = new
            {
                Description = description
            };

            // Serialize the metadata to a JSON string
            string metadataJson = JsonConvert.SerializeObject(metadata);

            // Create a new HTTP request with the file content and metadata as the body
            using (var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl))
            using (var content = new MultipartFormDataContent())
            {
                // Add the file content to the request body
                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "file", filename);

                // Add the metadata to the request body
                var metadataContent = new StringContent(metadataJson);
                metadataContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Add(metadataContent, "form-data");

                request.Content = content;

                // Send the HTTP request and get the response
                HttpResponseMessage response = await client.SendAsync(request);

                // Check if the upload and metadata update were successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded and metadata set successfully.");
                }
                else
                {
                    Console.WriteLine($"Upload failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }
            }
        }
    }
}
