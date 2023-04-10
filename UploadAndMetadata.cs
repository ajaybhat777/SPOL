using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            // Construct the URL to upload the file
            string uploadUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drives/{driveId}/root:/Images/{filename}:/content";

            // Read the file into a byte array
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Create a new HTTP request with the file content as the body
            using (var request = new HttpRequestMessage(HttpMethod.Put, uploadUrl))
            using (var content = new ByteArrayContent(fileBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                request.Content = content;

                // Send the HTTP request and get the response
                HttpResponseMessage response = await client.SendAsync(request);

                // Check if the upload was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded successfully.");

                    // Get the ID of the uploaded file
                    var responseBody = await response.Content.ReadAsStringAsync();
                    dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                    string fileId = responseJson.id;

                    // Construct the URL to update the metadata of the file
                    string metadataUrl = $"https://graph.microsoft.com/beta/sites/{siteId}/drives/{driveId}/items/{fileId}/fields";

                    // Define the metadata properties to update
                    var metadata = new
                    {
                        Description = description
                    };

                    // Serialize the metadata to a JSON string
                    string metadataJson = JsonConvert.SerializeObject(metadata);

                    // Create a new HTTP request to update the metadata
                    using (var metadataRequest = new HttpRequestMessage(new HttpMethod("PATCH"), metadataUrl))
                    {
                        metadataRequest.Content = new StringContent(metadataJson, Encoding.UTF8, "application/json");

                        // Send the HTTP request and get the response
                        HttpResponseMessage metadataResponse = await client.SendAsync(metadataRequest);

                        // Check if the metadata update was successful
                        if (metadataResponse.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Metadata updated successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Metadata update failed with status code {metadataResponse.StatusCode}: {await metadataResponse.Content.ReadAsStringAsync()}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Upload failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }
            }
        }
    }
}
